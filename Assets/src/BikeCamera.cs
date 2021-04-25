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

    void Awake()
    {
        nextpos = target.transform.position + initialoffset;
        transform.position = nextpos;

        initialoffsetr = transform.rotation;
        nextrot = Quaternion.LookRotation(target.transform.forward, Vector3.up) * initialoffsetr;
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
            Quaternion lookatend = Quaternion.LookRotation(lookattarget.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookatend, Time.deltaTime * rotationsmoothing);

            return;
        }

        // -- update camera position
        Quaternion behindplayerrot = Quaternion.Euler(0f, yawrot, 0f);
        nextpos = target.transform.position + (behindplayerrot * initialoffset);
        transform.position = Vector3.Lerp(transform.position, nextpos, Time.deltaTime * positionsmoothing);

        // -- update camera rotation
        Vector3 combinedlook = Vector3.up * (1 - playerlookweight) + (target.transform.position - transform.position).normalized * playerlookweight;
        nextrot = Quaternion.LookRotation(combinedlook, Vector3.up) * Quaternion.Euler(0f, 0f, biketilt) * initialoffsetr;
        transform.rotation = Quaternion.Slerp(transform.rotation, nextrot, Time.deltaTime * rotationsmoothing);
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
