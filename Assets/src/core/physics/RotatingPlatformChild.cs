using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatformChild : AttachablePlatform
{
    [SerializeField]
    private RotatingPlatform parent;
    [SerializeField]
    private bool counterparentrotation = true;
    [SerializeField]
    private Vector3 rotateaxis = Vector3.zero;
    [SerializeField]
    private float rotatespeed = 0f;

    private bool activated = true;

    protected override void OnAwake()
    {
        rotateaxis.Normalize();
        counteractscaling = false;
    }

    void FixedUpdate()
    {
        if(counterparentrotation)
            this.transform.Rotate(parent.RotationAxis(), -parent.RotationSpeed());
        if (activated)
            this.transform.Rotate(rotateaxis, rotatespeed);
    }

    public void Activate(bool activate)
    {
        this.activated = activate;
    }

    protected override void IOnCollisionEnter(Collision collision)
    {

    }

    protected override void IOnCollisionExit(Collision collision)
    {

    }
}
