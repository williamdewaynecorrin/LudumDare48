using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeCamera : MonoBehaviour
{
    [SerializeField]
    private BikePlayer target;
    [SerializeField]
    private Vector3 initialoffset;
    [SerializeField]
    private float offsetspringamt;

    [SerializeField]
    private float positionsmoothing = 10.0f;
    [SerializeField]
    private float rotationsmoothing = 10.0f;
    [Range(0f, 1f)]
    [SerializeField]
    private float playerlookweight = 0.5f;
    [SerializeField]
    private float biketiltmult = 0.2f;
    [SerializeField]
    private float flipvalmult = 0.4f;

    private Vector3 offsetmax, offsetmin;
    private Vector3 nextpos;

    private Quaternion nextrot;
    private Quaternion initialoffsetr;
    private float yawrot;
    private Vector3 playerfacing;

    private float biketilt;

    private bool lookatmode = false;
    private Vector3 lookattargetpos;
    private Vector3 lookattarget;

    private bool flipped = false;
    private int fliptimer = 0; 
    private int fliptimerreset = 30;

    void Awake()
    {
        nextpos = target.transform.position + initialoffset;
        transform.position = nextpos;

        initialoffsetr = transform.rotation;
        nextrot = Quaternion.LookRotation(target.transform.forward, PhysicsManager.NormalDir()) * initialoffsetr;
    }

    void FixedUpdate()
    {
        if(lookatmode)
        {
            // -- position
            Vector3 targetpos = lookattargetpos;
            transform.position = Vector3.Lerp(transform.position, targetpos, Time.deltaTime * positionsmoothing);

            // -- rotation
            Vector3 lookattarget = this.lookattarget - transform.position;
            Quaternion lookatend = Quaternion.LookRotation(lookattarget.normalized, PhysicsManager.NormalDir());
            transform.rotation = Quaternion.Slerp(transform.rotation, lookatend, Time.deltaTime * rotationsmoothing);

            return;
        }

        // -- handle flipped status
        if (flipped)
        {
            ++fliptimer;
            if (fliptimer == fliptimerreset)
            {
                fliptimer = 0;
                flipped = false;
            }
        }

        float flippedval = ((float)(fliptimerreset - fliptimer)) / ((float)(fliptimerreset));
        float flippedmult = flipped ? (1 - flippedval) * flipvalmult : 1;

        // -- calculate angle for gravity changes
        float gravityangle = -Vector3.Angle(PhysicsManager.DefaultGravityDir(), PhysicsManager.GravityDir());
        Quaternion gravityrot = Quaternion.AngleAxis(gravityangle, Vector3.forward);

        // -- update camera position
        Quaternion behindplayerrot = Quaternion.Euler(0f, yawrot, 0f) * gravityrot;
        nextpos = target.transform.position + (behindplayerrot * initialoffset);
        transform.position = Vector3.Lerp(transform.position, nextpos, Time.deltaTime * positionsmoothing * flippedmult);

        // -- update camera rotation
        Quaternion initialoffsetrrotated = initialoffsetr;
        Vector3 combinedlook = PhysicsManager.NormalDir() * (1 - playerlookweight) + (target.transform.position - transform.position).normalized * playerlookweight;
        nextrot = Quaternion.LookRotation(combinedlook, PhysicsManager.NormalDir()) * Quaternion.Euler(0f, 0f, biketilt + gravityangle) * Quaternion.Euler(0f, 0f, gravityangle) * initialoffsetrrotated;
        transform.rotation = Quaternion.Slerp(transform.rotation, nextrot, Time.deltaTime * rotationsmoothing * flippedmult);
    }

    public void SetBikeTilt(float biketilt)
    {
        this.biketilt = biketilt * -biketiltmult;
    }

    public void SetPlayerYAWOffset(float yawoffset)
    {
        yawrot = yawoffset;
    }

    public void SetPlayerFacing(Vector3 facing)
    {
        playerfacing = facing;
    }

    public void FlipGravity()
    {
        flipped = true;
        fliptimer = 0;
    }

    public void SetLookAtMode(Vector3 targetposition, Vector3 targetlookat)
    {
        lookatmode = true;
        lookattarget = targetlookat;
        lookattargetpos = targetposition;
    }

    public void SetRegularMode()
    {
        lookatmode = false;
    }

    public Vector3 MovementVectorRight()
    {
        Vector3 right = transform.right;
        right = new Vector3(right.x, 0f, right.z).normalized;

        return right;
    }

    public Vector3 MovementVectorForward()
    {
        Vector3 forward = transform.forward;
        forward = new Vector3(forward.x, 0f, forward.z).normalized;

        return forward;
    }
}
