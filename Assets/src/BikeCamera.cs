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

    private float biketilt;

    void Awake()
    {
        nextpos = target.transform.position + initialoffset;
        transform.position = nextpos;

        initialoffsetr = transform.rotation;
        nextrot = Quaternion.LookRotation(target.transform.forward, Vector3.up) * initialoffsetr;
    }

    void FixedUpdate()
    {
        // -- update camera position
        nextpos = target.transform.position + initialoffset;
        transform.position = Vector3.Lerp(transform.position, nextpos, Time.deltaTime * positionsmoothing);

        // -- update camera rotation
        Vector3 combinedlook = target.transform.forward * (1 - playerlookweight) + (target.transform.position - transform.position).normalized * playerlookweight;
        nextrot = Quaternion.LookRotation(combinedlook, Vector3.up) * Quaternion.Euler(0f, 0f, biketilt) * initialoffsetr;
        transform.rotation = Quaternion.Slerp(transform.rotation, nextrot, Time.deltaTime * rotationsmoothing);
    }

    public void SetBikeTilt(float biketilt)
    {
        this.biketilt = biketilt * -biketiltmult;
    }
}
