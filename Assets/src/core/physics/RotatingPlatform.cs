using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : AttachablePlatform
{
    [SerializeField]
    private Vector3 rotateaxis = Vector3.up;
    [SerializeField]
    private float rotatespeed = 10f;

    private bool activated = true;

    protected override void OnAwake()
    {
        rotateaxis.Normalize();
    }

    void FixedUpdate()
    {
        if(activated)
            this.transform.Rotate(rotateaxis, rotatespeed);
    }

    public void Activate(bool activate)
    {
        this.activated = activate;
    }

    public Vector3 RotationAxis()
    {
        return rotateaxis;
    }

    public float RotationSpeed()
    {
        return rotatespeed;
    }

    protected override void IOnCollisionEnter(Collision collision)
    {

    }

    protected override void IOnCollisionExit(Collision collision)
    {

    }
}
