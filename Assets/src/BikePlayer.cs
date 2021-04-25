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
    [SerializeField]
    private float tiltacceleration = 0.01f;
    [SerializeField]
    private float airmovementmult = 0.075f;

    [Header("Interact Values")]
    [SerializeField]
    private float jumpstrength = 2.0f;
    [SerializeField]
    private float bouncerjumpmult = 4.0f;
    [SerializeField]
    private float boostacceleration = 0.01f;
    [SerializeField]
    private float maxboost = 0.5f;
    [SerializeField]
    private float booststopfriction = 0.8f;

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

    [Header("FX")]
    [SerializeField]
    private GameObject bikeparticlesprefab;
    [SerializeField]
    private Transform bikeparticlestransform;
    [SerializeField]
    private int bikeparticleemitframe = 4;
    [SerializeField]
    private GameObject boostparticlesprefab;
    [SerializeField]
    private int boostparticleemitframe = 4;

    private int bikeparticleemitframetimer = 0;
    private int boostparticleemitframetimer = 0;

    [Header("SFX")]
    [SerializeField]
    private SmartAudio bikeaudio;
    [SerializeField]
    private SmartAudio boostaudio;
    [SerializeField]
    private AudioClip sfxthruststart;
    [SerializeField]
    private AudioClip sfxthrustloop;
    [SerializeField]
    private AudioClip sfxboostloop;
    [SerializeField]
    private AudioClip sfxdeathfall;
    [SerializeField]
    private AudioClip[] sfxwallbumps;
    [SerializeField]
    private AudioClip sfxjump;

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
    private Vector3 lasttiltvelocity;
    private Vector3 lookdirection;
    private Vector2 framemovementinput;
    private bool drifting = false;
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
    private float speedratio;

    private bool jumped = false;
    private bool boosting = false;
    private Vector3 boostdir;
    private Vector3 currentboostvelocity;

    private bool debugconsole = false;

    //==================================================================================================================================================
    // -- Initialization methods
    //==================================================================================================================================================
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        lookdirection = bikemesh.transform.forward;
        bikeuprightrot = bikemesh.transform.eulerAngles;
        bikeaudio.SetClip(sfxthruststart);
    }

    //==================================================================================================================================================
    // -- Update / input methods
    //==================================================================================================================================================
    void Update()
    {
        framemovementinput = GetMovementInput();
        framehasinput = framemovementinput != Vector2.zero;
        drifting = Input.GetKey(KeyCode.LeftShift);

        currentbiketilt = Mathf.Lerp(currentbiketilt, targetbiketilt, Time.deltaTime * biketiltsmoothing);
        camera.SetBikeTilt(currentbiketilt);

        // -- bike rot
        Quaternion targetrotation = Quaternion.LookRotation(lookdirection, Vector3.up) * Quaternion.Euler(Vector3.forward * currentbiketilt) * Quaternion.Euler(bikeuprightrot);
        bikemesh.transform.rotation = Quaternion.Slerp(bikemesh.transform.rotation, targetrotation, rotationsmoothing * Time.deltaTime);

        if (reversing)
            bikemesh.transform.localEulerAngles = bikemesh.transform.localEulerAngles.SetY(90f);

        if (Input.GetKeyDown(KeyCode.Tilde))
            debugconsole = !debugconsole;
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
        // -- construct rotation vector for rotating front and back tire colliders to match YAW tilt
        Vector3 velnoy = currentvelocity.NoY().normalized;
        Quaternion bikemeshrotate = Quaternion.LookRotation(velnoy, Vector3.up);
        camera.SetPlayerYAWOffset(bikemeshrotate.eulerAngles.y);
        camera.SetPlayerFacing(velnoy);

        // -- get wheel colliders and centers
        SphereCollider frontwheelcol = frontwheel.GetCollider();
        SphereCollider backwheelcol = backwheel.GetCollider();

        Vector3 frontwheellocal = bikemeshrotate * frontwheelcol.transform.localPosition;
        Vector3 backwheellocal = bikemeshrotate * backwheelcol.transform.localPosition;

        frontspherecenter = transform.position + frontwheellocal + frontwheelcol.center;
        backspherecenter = transform.position + backwheellocal + backwheelcol.center;

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

            if (groundedstatus == EWheelSelector.Back)
                PositionPlayerOnPoint(backspherecenter, backwheelcol, backgroundhit, backwheellocal);
            else
                PositionPlayerOnPoint(frontspherecenter, frontwheelcol, frontgroundhit, frontwheellocal);

            rigidbody.velocity = Vector3.zero;

            // -- movement logic
            MovementLogic(1.0f);
        }
        else
        {
            // -- air movement logic
            AirMovementLogic();

            // -- add gravity
            rigidbody.velocity += PhysicsManager.gravity;
        }

        BoostLogic();

        reversing = Vector3.Dot(currentvelocity.normalized, camera.MovementVectorForward()) < 0f;

        transform.position += currentvelocity + currentboostvelocity;
        wasgrounded = grounded;

        // -- set speedometer ui
        speedratio = (currentvelocity.magnitude / maxspeed);
        float fullratio = speedratio * 0.7f + (currentboostvelocity.magnitude / maxboost) * 0.3f;
        uispeedometer.SetSpeedRatio(fullratio);
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
            SFXManager.PlayRandomClip2D(sfxwallbumps);
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

                wallstatus = EWheelSelector.Back;
                SFXManager.PlayRandomClip2D(sfxwallbumps);
            }
            else
                wallstatus = EWheelSelector.Both;
        }

        if (wallstatus == EWheelSelector.Neither)
            runningintowall = false;
        else
            runningintowall = true;
    }

    private void MovementLogic(float stdmult)
    {
        Vector3 right = camera.MovementVectorRight();
        Vector3 forward = camera.MovementVectorForward();

        // -- accel
        Vector3 framevelocity = CalculateWheelAccel(forward, right, out bool onlytilt);
        if (framevelocity == Vector3.zero || onlytilt)
        {
            float stopmult = 1f;
            float tiltmult = drifting ? 2.0f : 1.0f;
            if (onlytilt)
            {
                currentvelocity += lasttiltvelocity * tiltmult * stdmult;
                stopmult = 0.975f;
            }

            currentvelocity *= stopfriction * stopmult * stdmult;
            currentmotortorque *= stopfrictionmotortorque;
            bikeaudio.BeginStop(45);
        }
        else
        {
            Vector3 velocityadd = framevelocity * stdmult;
            if(drifting)
            {
                velocityadd = framevelocity * 0.5f + lasttiltvelocity * 1.2f * stdmult;
            }

            currentvelocity += velocityadd;
            currentvelocity = Vector3.ClampMagnitude(currentvelocity, maxspeed);
            currentmotortorque = Mathf.Clamp(currentmotortorque, -maxmotortorque, maxmotortorque);

            if (bikeaudio.State() == ESmartAudioState.Stopped || bikeaudio.State() == ESmartAudioState.Stopping)
                bikeaudio.BeginStart(sfxthrustloop, true, 45);

            EmitThrustParticles(speedratio);
        }

        // -- tilt
        Vector3 tiltvelocity = CalculateBikeTilt(right * stdmult);
        if (tiltvelocity == Vector3.zero)
        {
            targetbiketilt *= stopfrictionbiketilt;
        }
        else
        {
            float tiltmax = drifting ? lateraltiltmax * 1.4f : lateraltiltmax;
            targetbiketilt = Mathf.Clamp(targetbiketilt, -tiltmax, tiltmax);
        }
    }

    private void AirMovementLogic()
    {
        MovementLogic(airmovementmult * Mathf.Clamp(rigidbody.velocity.magnitude, 0.1f, 10.0f));
    }

    private void BoostLogic()
    {
        Vector3 forward = boostdir;

        // -- calculate velocity
        Vector3 framevelocity = CalculateBoostAccel(forward);

        if (!boosting)
        {
            currentboostvelocity *= booststopfriction;
            if(boostaudio.State() == ESmartAudioState.Started || boostaudio.State() == ESmartAudioState.Starting)
                boostaudio.BeginStop(45);
        }
        else
        {
            currentboostvelocity += framevelocity;
            currentboostvelocity = Vector3.ClampMagnitude(currentboostvelocity, maxboost);

            if (boostaudio.State() == ESmartAudioState.Stopped || boostaudio.State() == ESmartAudioState.Stopping)
                boostaudio.BeginStart(sfxboostloop, true, 45);

            EmitBoostParticles();
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

    private Vector3 CalculateWheelAccel(Vector3 forward, Vector3 right, out bool onlytilt)
    {
        Vector3 framemovement = forward * framemovementinput.y;
        framemovement *= acceleration;

        Vector3 biketiltadd = right * framemovementinput.x;
        biketiltadd *= Mathf.Abs(currentbiketilt) * tiltacceleration;
        lasttiltvelocity = biketiltadd;
        //biketiltadd = Quaternion.Euler(0f, -currentbiketilt * tiltacceleration, 0f) * framemovement * tiltacceleration;

        // -- apply torque to all wheels
        currentmotortorque += framemovementinput.y * basetorque;
        frontwheel.SetTorque(currentmotortorque);
        backwheel.SetTorque(currentmotortorque);

        onlytilt = framemovement == Vector3.zero && biketiltadd != Vector3.zero;

        return framemovement + biketiltadd;
    }

    private Vector3 CalculateBikeTilt(Vector3 right)
    {
        Vector3 framemovement = right * framemovementinput.x;

        // -- apply tilt to bike to all wheels
        targetbiketilt += framemovementinput.x * -lateraltiltamt;

        return framemovement;
    }

    private Vector3 CalculateBoostAccel(Vector3 forward)
    {
        Vector3 framemovement = boosting ? forward : Vector3.zero;
        framemovement *= boostacceleration;

        // -- apply torque to all wheels
        currentmotortorque += 0.1f * basetorque;
        frontwheel.SetTorque(currentmotortorque);
        backwheel.SetTorque(currentmotortorque);

        return framemovement;
    }

    private void OnGroundLand()
    {
        jumped = false;
    }

    private void PositionPlayerOnPoint(Vector3 spherecenter, SphereCollider spherecol, RaycastHit hit, Vector3 spherelocal)
    {
        // -- lets take the middle between the two spheres as a reference point, instead of only one
        Vector3 pointonsphere = spherecenter + (hit.point - spherecenter).normalized * spherecol.radius;
        Vector3 pointtobot = (spherecenter + Vector3.down * spherecol.radius) - pointonsphere;

        Vector3 spherecasterbump = -spherelocal - spherecol.center * 1.01f + Vector3.up * spherecol.radius * 1.01f;
        transform.position = spherecasterbump + hit.point + pointtobot;
    }

    private void EmitThrustParticles(float scale)
    {
        ++bikeparticleemitframetimer;
        if(bikeparticleemitframetimer == bikeparticleemitframe)
        {
            bikeparticleemitframetimer = 0;
            GameObject particleinstance = GameObject.Instantiate(bikeparticlesprefab, bikeparticlestransform.position, bikeparticlestransform.rotation);
            particleinstance.transform.localScale *= Mathf.Clamp(scale, 0.25f, 1.0f);
            GameObject.Destroy(particleinstance.gameObject, 1.2f);
        }
    }

    private void EmitBoostParticles()
    {
        ++boostparticleemitframetimer;
        if (boostparticleemitframetimer == boostparticleemitframe)
        {
            boostparticleemitframetimer = 0;
            GameObject particleinstance = GameObject.Instantiate(boostparticlesprefab, bikeparticlestransform.position, bikeparticlestransform.rotation);
            particleinstance.transform.localScale *= Mathf.Clamp(1.0f, 0.25f, 1.0f);
            GameObject.Destroy(particleinstance.gameObject, 1.2f);
        }
    }

    private void Jump(float strength)
    {
        Vector3 jumpforce = Vector3.up * strength;
        rigidbody.velocity = jumpforce;
        transform.position += (jumpforce) * 0.2f;
        SFXManager.PlayClip2D(sfxjump);

        jumped = true;
    }

    private void FinishLevel(string nextlevel)
    {

    }

    private void Boost(Vector3 boostdir)
    {
        this.boostdir = boostdir;
    }

    //==================================================================================================================================================
    // -- Unity event methods
    //==================================================================================================================================================
    private void OnTriggerEnter(Collider other)
    {
        Bouncer bouncer = other.GetComponent<Bouncer>();
        if(bouncer != null)
        {
            Jump(jumpstrength * bouncerjumpmult);
        }

        Finish finish = other.GetComponent<Finish>();
        if(finish != null)
        {
            FinishLevel(finish.nextlevel);
        }

        BoostZone boostzone = other.GetComponent<BoostZone>();
        if(boostzone != null)
        {
            Boost(boostzone.GetBoostDirection());
            boosting = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        BoostZone boostzone = other.GetComponent<BoostZone>();
        if (boostzone != null)
        {
            boosting = false;
        }
    }

    //==================================================================================================================================================
    // -- Debug methods
    //==================================================================================================================================================
    void OnGUI()
    {
        if (!debugconsole)
            return;

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
