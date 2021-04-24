using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikePlayer : MonoBehaviour
{
    //==================================================================================================================================================
    // -- Properties
    //==================================================================================================================================================
    [Header("Wheels")]
    [SerializeField]
    private BikeWheel frontwheelmesh;
    [SerializeField]
    private BikeWheel backwheelmesh;
    [SerializeField]
    private float maxmotortorque = 1000f;
    [SerializeField]
    private float stopfrictionmotortorque = 0.8f;

    [Header("Physics Values")]
    [SerializeField]
    private float basetorque = 200f;
    [SerializeField]
    private float acceleration = 1.0f;
    [SerializeField]
    private float stopfriction = 0.7f;
    [SerializeField] 
    private float maxspeed = 0.2f;
    [SerializeField]
    private float lateralmult = 0.075f;

    [Header("Ground Detection")]
    [SerializeField]
    private SphereCollider groundsphere;
    [SerializeField]
    private LayerMask groundmask;

    [Header("Wall Detection")]
    [SerializeField]
    private SphereCollider wallsphere;
    [SerializeField]
    private LayerMask wallmask;

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
    private Vector3 tirecolliderinitiallocalpos;
    private float currentmotortorque;
    private Vector3 bikeuprightrot;

    private Vector3 spherecenter;
    private float castdist;
    private ECurrentGroundStatus groundstatus = ECurrentGroundStatus.InAir;
    private Vector3 wallspherecenter;
    private float wallcastdist;
    private Vector3 lastwallbinormal;
    private bool runningintowall = false;

    //==================================================================================================================================================
    // -- Initialization methods
    //==================================================================================================================================================
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        lookdirection = transform.forward;
        bikeuprightrot = transform.eulerAngles;
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

        if (framehasinput)
        {                                                                       
            Vector3 weightedlookdir = lookdirection * (1f - bikeuprightbias) + bikeuprightrot * bikeuprightbias;
            Quaternion targetrotation = Quaternion.LookRotation(weightedlookdir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetrotation, rotationsmoothing * Time.deltaTime);
        }
    }

    private Vector2 GetMovementInput()
    {
        float xkeyboardin = Input.GetKey(KeyCode.D) ? 1.0f : (Input.GetKey(KeyCode.A) ? -1.0f : 0.0f);
        //float xcontrollerin = Input.GetAxis("Horizontal");
        float xin = Mathf.Clamp(xkeyboardin, -1f, 1f);

        float ykeyboardin = Input.GetKey(KeyCode.W) ? 1.0f : (Input.GetKey(KeyCode.S) ? -1.0f : 0.0f);
        //float ycontrollerin = Input.GetAxis("Vertical");
        float yin = Mathf.Clamp(ykeyboardin, -1f, 1f);

        return new Vector2(xin, yin);
    }

    //==================================================================================================================================================
    // -- Fixed update / physics methods
    //==================================================================================================================================================
    void FixedUpdate()
    {
        // -- check for walls
        WallCheck(out RaycastHit wallhit);

        // -- check for grounded
        GroundedCheck(out RaycastHit groundhit);

        if (grounded)
        {
            if(!wasgrounded)
            {
                OnGroundLand();
            }

            rigidbody.velocity = Vector3.zero;
            PositionPlayerOnPoint(spherecenter, groundhit);

            // -- movement logic
            Vector3 right = Vector3.right; // change to camera.right later
            Vector3 forward = Vector3.forward; // change to camera.forward later

            Vector3 framevelocity = CalculateWheelAccel(right * lateralmult, forward);
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
        }
        else
        {
            // -- gravity logic
            rigidbody.velocity += PhysicsManager.gravity;
        }

        transform.position += currentvelocity;
        if (currentvelocity != Vector3.zero)
            lookdirection = currentvelocity.normalized;

        wasgrounded = grounded;
    }

    private void GroundedCheck(out RaycastHit groundhit)
    {
        Vector3 direction = Vector3.down;
        castdist = Mathf.Max(groundsphere.radius * 1.01f, rigidbody.velocity.OnlyY().magnitude * magicgroundnumber);
        spherecenter = groundsphere.transform.position + groundsphere.center;

        if (Physics.SphereCast(spherecenter, groundsphere.radius, direction, out groundhit, castdist, groundmask, QueryTriggerInteraction.Ignore))
        {
            grounded = true;

            // -- do angle test later
            float angle = Mathf.Abs(Vector3.Angle(groundhit.normal, Vector3.up));
            groundstatus = ECurrentGroundStatus.Grounded;
        }
        else
        {
            grounded = false;
            groundstatus = ECurrentGroundStatus.InAir;
        }
    }

    private void WallCheck(out RaycastHit wallhit)
    {
        Vector3 direction = currentvelocity.normalized;
        wallcastdist = Mathf.Max(wallsphere.radius * 1.01f, currentvelocity.magnitude * magicwallnumber);
        wallspherecenter = wallsphere.transform.position + wallsphere.center;

        if (Physics.SphereCast(wallspherecenter, wallsphere.radius, direction, out wallhit, wallcastdist, wallmask, QueryTriggerInteraction.Ignore))
        {
            Vector3 wallnormal = wallhit.normal;
            Vector3 temp = Vector3.Cross(wallnormal, currentvelocity.normalized);
            Vector3 wallbinormal = Vector3.Cross(temp, wallnormal);
            lastwallbinormal = wallbinormal;

            currentvelocity = Vector3.Project(currentvelocity, wallbinormal);
            transform.position += wallnormal * 0.01f;

            // -- do angle test later
            float angle = Mathf.Abs(Vector3.Angle(wallnormal, Vector3.up));

            runningintowall = true;
        }
        else
            runningintowall = false;
    }

    private Vector3 CalculateWheelAccel(Vector3 right, Vector3 forward)
    {
        Vector3 framemovement = forward * framemovementinput.y;
        framemovement *= acceleration;

        // -- apply torque to all wheels
        currentmotortorque += framemovementinput.y * basetorque;
        frontwheelmesh.SetTorque(currentmotortorque);
        backwheelmesh.SetTorque(currentmotortorque);

        return framemovement;
    }

    private void OnGroundLand()
    {

    }

    private void PositionPlayerOnPoint(Vector3 spherecenter, RaycastHit hit)
    {
        Vector3 pointonsphere = spherecenter + (hit.point - spherecenter).normalized * groundsphere.radius;
        Vector3 pointtobot = (spherecenter + Vector3.down * groundsphere.radius) - pointonsphere;

        Vector3 spherecasterbump = -groundsphere.transform.localPosition - groundsphere.center * 1.01f + Vector3.up * groundsphere.radius * 1.01f;
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
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        
        // -- red is for ground
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spherecenter, groundsphere.radius);
        Gizmos.DrawLine(spherecenter, spherecenter + Vector3.down * castdist);

        // -- green is for walls
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(wallspherecenter, wallsphere.radius);
        Gizmos.DrawLine(wallspherecenter, wallspherecenter + currentvelocity.normalized * wallcastdist);
    }
}

public enum ECurrentGroundStatus
{
    InAir = 0,
    Grounded = 1,
    OnRamp = 2
}
