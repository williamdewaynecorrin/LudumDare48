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

    private Vector3 offsetmax, offsetmin;
    private Vector3 nextpos;

    private Quaternion nextrot;
    private Quaternion initialoffsetr;

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
        nextrot = Quaternion.LookRotation(target.transform.forward, Vector3.up) * initialoffsetr;
        transform.rotation = Quaternion.Slerp(transform.rotation, nextrot, Time.deltaTime * rotationsmoothing);
    }
}
