using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikePlayer : MonoBehaviour
{
    //==================================================================================================================================================
    // -- Properties
    //==================================================================================================================================================
    [Header("Wheels & Bike")]
    [SerializeField]
    private Transform bikemesh;
    [SerializeField]
    private BikeWheel frontwheel;
    [SerializeField]
    private BikeWheel backwheel;
    [SerializeField]
    private float maxmotortorque = 1000f;
    [SerializeField]
    private float stopfrictionmotortorque = 0.8f;
    [SerializeField]
    private new BikeCamera camera;

    [Header("Physics Values")]
    [SerializeField]
    private float basetorque = 200f;
    [SerializeField]
    private float acceleration = 1.0f;
    [SerializeField]
    private float stopfriction = 0.7f;
    [SerializeField] 
    private float maxspeed = 0.2f;

    [Header("Bike Tilting")]
    [SerializeField]
    private float lateralacceleration = 0.075f;
    [SerializeField]
    private float lateraltiltamt = 1f;
    [SerializeField]
    private float lateraltiltmax = 45f;
    [SerializeField]
    private float biketiltsmoothing = 10.0f;
    [SerializeField]
    private float stopfrictionbiketilt = 0.9f;
    
    [Header("Ground Detection")]
    [SerializeField]
    private LayerMask groundmask;

    [Header("Wall Detection")]
    [SerializeField]
    private LayerMask wallmask;

    [Header("UI Connections")]
    [SerializeField]
    private UISpeedometer uispeedometer;

    [SerializeField]
    private float rotationsmoothing = 10.0f;
    [Range(0f, 1f)]
    [SerializeField]
    private float bikeuprightbias = 0.6f;

    [Range(-10f, 10f)]
    [SerializeField]
    private float magicgroundnumber = 1.0f;
    [Range(-10f, 10f)]
    [SerializeField]
    private float magicwallnumber = 1.0f;
    
    private new Rigidbody rigidbody;
    private Vector3 currentvelocity;
    private Vector3 lookdirection;
    private Vector2 framemovementinput;
    private bool grounded = false;
    private bool wasgrounded = false;
    private bool framehasinput = false;
    private bool reversing = false;
    private Vector3 tirecolliderinitiallocalpos;
    private float currentmotortorque;
    private Vector3 bikeuprightrot;
    private float targetbiketilt;
    private float currentbiketilt;

    private Vector3 frontspherecenter;
    private float frontcastgrounddist;
    private float frontcastwalldist;

    private Vector3 backspherecenter;
    private float backcastgrounddist;
    private float backcastwalldist;

    private EWheelSelector groundedstatus = EWheelSelector.Neither;
    private EWheelSelector wallstatus = EWheelSelector.Neither;
    private Vector3 lastwallbinormal;
    private bool runningintowall = false;

    //==================================================================================================================================================
    // -- Initialization methods
    //==================================================================================================================================================
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        lookdirection = bikemesh.transform.forward;
        bikeuprightrot = bikemesh.transform.eulerAngles;
    }

    void Start()
    {

    }

    //==================================================================================================================================================
    // -- Update / input methods
    //==================================================================================================================================================
    void Update()
    {
        framemovementinput = GetMovementInput();
        framehasinput = framemovementinput != Vector2.zero;

        currentbiketilt = Mathf.Lerp(currentbiketilt, targetbiketilt, Time.deltaTime * biketiltsmoothing);
        camera.SetBikeTilt(currentbiketilt);

        //if (framehasinput)
        //{
            // -- bike rot
            Quaternion targetrotation = Quaternion.LookRotation(lookdirection, Vector3.up) * Quaternion.Euler(Vector3.forward * currentbiketilt) * Quaternion.Euler(bikeuprightrot);
            bikemesh.transform.rotation = Quaternion.Slerp(bikemesh.transform.rotation, targetrotation, rotationsmoothing * Time.deltaTime);

            if (reversing)
                bikemesh.transform.localEulerAngles = bikemesh.transform.localEulerAngles.SetY(90f);
        //}
    }

    private Vector2 GetMovementInput()
    {
        float xkeyboardin = Input.GetKey(KeyCode.D) ? 1.0f : (Input.GetKey(KeyCode.A) ? -1.0f : 0.0f);
        float xin = Mathf.Clamp(xkeyboardin, -1f, 1f);

        float ykeyboardin = Input.GetKey(KeyCode.W) ? 1.0f : (Input.GetKey(KeyCode.S) ? -1.0f : 0.0f);
        float yin = Mathf.Clamp(ykeyboardin, -1f, 1f);

        return new Vector2(xin, yin);
    }

    //==================================================================================================================================================
    // -- Fixed update / physics methods
    //==================================================================================================================================================
    void FixedUpdate()
    {
        // -- get wheel colliders and centers
        SphereCollider frontwheelcol = frontwheel.GetCollider();
        SphereCollider backwheelcol = backwheel.GetCollider();
        frontspherecenter = frontwheelcol.transform.position + frontwheelcol.center;
        backspherecenter = backwheelcol.transform.position + backwheelcol.center;

        // -- check for walls
        WallCheck(frontwheelcol, backwheelcol, out RaycastHit frontwallhit, out RaycastHit backwallhit);

        // -- check for grounded
        GroundedCheck(frontwheelcol, backwheelcol, out RaycastHit frontgroundhit, out RaycastHit backgroundhit);

        if (grounded)
        {
            if(!wasgrounded)
            {
                OnGroundLand();
            }

            rigidbody.velocity = Vector3.zero;

            if(groundedstatus == EWheelSelector.Back)
                PositionPlayerOnPoint(backspherecenter, backwheelcol, backgroundhit);
            else
                PositionPlayerOnPoint(frontspherecenter, frontwheelcol, frontgroundhit);

            // -- movement logic
            MovementLogic();
        }
        else
        {
            // -- gravity logic
            rigidbody.velocity += PhysicsManager.gravity;
        }

        reversing = Vector3.Dot(currentvelocity.normalized, transform.forward) < 0f;

        transform.position += currentvelocity;
        wasgrounded = grounded;

        // -- set speedometer ui
        float speedratio = currentvelocity.magnitude / maxspeed;
        uispeedometer.SetSpeedRatio(speedratio);
    }

    private void GroundedCheck(SphereCollider frontcol, SphereCollider backcol, out RaycastHit frontgroundhit, out RaycastHit backgroundhit)
    {
        // -- reset grounded status
        groundedstatus = EWheelSelector.Neither;

        Vector3 direction = Vector3.down;
        frontcastgrounddist = Mathf.Max(frontcol.radius * 1.01f, rigidbody.velocity.OnlyY().magnitude * magicgroundnumber);
        backcastgrounddist = Mathf.Max(backcol.radius * 1.01f, rigidbody.velocity.OnlyY().magnitude * magicgroundnumber);

        // -- front wheel first
        if (Physics.SphereCast(frontspherecenter, frontcol.radius, direction, out frontgroundhit, frontcastgrounddist, groundmask, QueryTriggerInteraction.Ignore))
        {
            // -- do angle test later
            float angle = Mathf.Abs(Vector3.Angle(frontgroundhit.normal, Vector3.up));
            groundedstatus = EWheelSelector.Front;

            lookdirection = BinormalFromHitNormal(frontgroundhit.normal).normalized;
        }

        // -- then back wheel
        if (Physics.SphereCast(backspherecenter, backcol.radius, direction, out backgroundhit, backcastgrounddist, groundmask, QueryTriggerInteraction.Ignore))
        {
            // -- do angle test later
            float angle = Mathf.Abs(Vector3.Angle(backgroundhit.normal, Vector3.up));
            groundedstatus = groundedstatus == EWheelSelector.Front ? 
                             EWheelSelector.Both : EWheelSelector.Back;

            // -- things to only do once per collision here
            if (groundedstatus == EWheelSelector.Back)
            {
                lookdirection = BinormalFromHitNormal(backgroundhit.normal).normalized;
            }
        }

        if (groundedstatus == EWheelSelector.Neither)
            grounded = false;
        else
            grounded = true;
    }

    private void WallCheck(SphereCollider frontcol, SphereCollider backcol, out RaycastHit frontwallhit, out RaycastHit backwallhit)
    {
        // -- reset wall status
        wallstatus = EWheelSelector.Neither;

        Vector3 direction = currentvelocity.normalized;
        frontcastwalldist = Mathf.Max(frontcol.radius * 1.01f, currentvelocity.magnitude * magicwallnumber);
        backcastwalldist = Mathf.Max(backcol.radius * 1.01f, currentvelocity.magnitude * magicwallnumber);

        // -- front wheel first
        if (Physics.SphereCast(frontspherecenter, frontcol.radius, direction, out frontwallhit, frontcastwalldist, wallmask, QueryTriggerInteraction.Ignore))
        {
            ProjectVelocity(frontwallhit.normal);
            transform.position += frontwallhit.normal * 0.01f;

            // -- do angle test later
            float angle = Mathf.Abs(Vector3.Angle(frontwallhit.normal, Vector3.up));

            wallstatus = EWheelSelector.Front;
        }

        // -- then back wheel
        if (Physics.SphereCast(backspherecenter, backcol.radius, direction, out backwallhit, backcastwalldist, wallmask, QueryTriggerInteraction.Ignore))
        {
            // -- only project velocity if we didn't just do it
            if (wallstatus == EWheelSelector.Neither)
            {
                ProjectVelocity(backwallhit.normal);
                transform.position += backwallhit.normal * 0.01f;

                // -- do angle test later
                float angle = Mathf.Abs(Vector3.Angle(frontwallhit.normal, Vector3.up));

                runningintowall = true;
                wallstatus = EWheelSelector.Back;
            }
            else
                wallstatus = EWheelSelector.Both;
        }

        if (wallstatus == EWheelSelector.Neither)
            runningintowall = false;
        else
            runningintowall = true;
    }

    private void MovementLogic()
    {
        Vector3 right = Vector3.right; // change to camera.right later
        Vector3 forward = Vector3.forward; // change to camera.forward later

        // -- accel
        Vector3 framevelocity = CalculateWheelAccel(forward);
        if (framevelocity == Vector3.zero)
        {
            currentvelocity *= stopfriction;
            currentmotortorque *= stopfrictionmotortorque;
        }
        else
        {
            currentvelocity += framevelocity;
            currentvelocity = Vector3.ClampMagnitude(currentvelocity, maxspeed);
            currentmotortorque = Mathf.Clamp(currentmotortorque, -maxmotortorque, maxmotortorque);
        }

        // -- tilt
        Vector3 tiltvelocity = CalculateBikeTilt(right);
        if (tiltvelocity == Vector3.zero)
        {
            targetbiketilt *= stopfrictionbiketilt;
        }
        else
        {
            targetbiketilt = Mathf.Clamp(targetbiketilt, -lateraltiltmax, lateraltiltmax);
        }
    }

    private void ProjectVelocity(Vector3 hitnormal)
    {
        Vector3 wallbinormal = BinormalFromHitNormal(hitnormal);
        lastwallbinormal = wallbinormal;

        currentvelocity = Vector3.Project(currentvelocity, wallbinormal);
    }

    private Vector3 BinormalFromHitNormal(Vector3 hitnormal)
    {
        Vector3 temp = Vector3.Cross(hitnormal, currentvelocity.normalized);
        return Vector3.Cross(temp, hitnormal);
    }

    private Vector3 CalculateWheelAccel(Vector3 forward)
    {
        Vector3 framemovement = forward * framemovementinput.y;
        framemovement *= acceleration;

        // -- apply torque to all wheels
        currentmotortorque += framemovementinput.y * basetorque;
        frontwheel.SetTorque(currentmotortorque);
        backwheel.SetTorque(currentmotortorque);

        return framemovement;
    }

    private Vector3 CalculateBikeTilt(Vector3 right)
    {
        Vector3 framemovement = right * framemovementinput.x;
        framemovement *= lateralacceleration;

        // -- apply tilt to bike to all wheels
        targetbiketilt += framemovementinput.x * -lateraltiltamt;

        return framemovement;
    }

    private void OnGroundLand()
    {

    }

    private void PositionPlayerOnPoint(Vector3 spherecenter, SphereCollider spherecol, RaycastHit hit)
    {
        Vector3 pointonsphere = spherecenter + (hit.point - spherecenter).normalized * spherecol.radius;
        Vector3 pointtobot = (spherecenter + Vector3.down * spherecol.radius) - pointonsphere;

        Vector3 spherecasterbump = -spherecol.transform.localPosition - spherecol.center * 1.01f + Vector3.up * spherecol.radius * 1.01f;
        transform.position = spherecasterbump + hit.point + pointtobot;
    }

    //==================================================================================================================================================
    // -- Debug methods
    //==================================================================================================================================================
    void OnGUI()
    {
        float debuglineheight = 26f;
        Rect baserect = new Rect(10f, 10f, 200f, 25f);

        GUI.TextField(baserect, string.Format("Movement Input: {0}", framemovementinput));
        baserect.y += debuglineheight;

        GUI.TextField(baserect, string.Format("Grounded?: {0}", grounded));
        baserect.y += debuglineheight;

        GUI.TextField(baserect, string.Format("Running Into Wall?: {0}", runningintowall));
        baserect.y += debuglineheight;

        GUI.TextField(baserect, string.Format("Motor Torque: {0}", currentmotortorque));
        baserect.y += debuglineheight;

        GUI.TextField(baserect, string.Format("Reversing?: {0}", reversing));
        baserect.y += debuglineheight;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        SphereCollider frontcol = frontwheel.GetCollider();
        SphereCollider backcol = backwheel.GetCollider();

        // -- red is for wheels
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(frontspherecenter, frontcol.radius);
        Gizmos.DrawWireSphere(backspherecenter, backcol.radius);

        // -- red is also for grounded lines
        Gizmos.DrawLine(frontspherecenter, frontspherecenter + Vector3.down * frontcastgrounddist);
        Gizmos.DrawLine(backspherecenter, backspherecenter + Vector3.down * backcastgrounddist);

        // -- green is for wall lines
        Gizmos.color = Color.green;
        Gizmos.DrawLine(frontspherecenter, frontspherecenter + currentvelocity.normalized * frontcastwalldist);
        Gizmos.DrawLine(backspherecenter, backspherecenter + currentvelocity.normalized * backcastwalldist);
    }
}

public enum EWheelSelector
{
    Neither = 0,
    Front = 1,
    Back = 2,
    Both = 3
}
