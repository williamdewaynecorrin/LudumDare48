using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : AttachablePlatform
{
    private static float targetreachedthresh = 0.01f; 
    [SerializeField]
    private Transform destinationtransform;
    [SerializeField]
    private float movespeed = 0.1f;
    [SerializeField]
    private Timer pausetime;

    private StoredTransform starttransformdata;
    private StoredTransform desttransformdata;
    private MovingPlatformState state = MovingPlatformState.TowardsStart;
    private Vector3 movedir = Vector3.zero;
    private Vector3 velocity = Vector3.zero;

    protected override void OnAwake()
    {
        starttransformdata = new StoredTransform(this.transform);
        desttransformdata = new StoredTransform(destinationtransform);
        pausetime.Init();

        GameObject.Destroy(destinationtransform.gameObject);
    }

    protected override void OnStart()
    {

    }

    void FixedUpdate()
    {
        Vector3 currenttarget = Vector3.zero;
        switch (state)
        {
            case MovingPlatformState.TowardsDestination:
                movedir = (desttransformdata.position - starttransformdata.position).normalized;
                currenttarget = desttransformdata.position;
                break;
            case MovingPlatformState.TowardsStart:
                movedir = (starttransformdata.position - desttransformdata.position).normalized;
                currenttarget = starttransformdata.position;
                break;
            case MovingPlatformState.Paused:
                pausetime.Decrement();
                if(pausetime.TimerReached())
                {
                    pausetime.Reset();
                    if (transform.position == desttransformdata.position)
                        state = MovingPlatformState.TowardsStart;
                    else if (transform.position == starttransformdata.position)
                        state = MovingPlatformState.TowardsDestination;
                }
                return;
        }

        velocity = movedir * movespeed;
        this.transform.position += velocity;

        // -- see if we have reached or surpassed target
        Vector3 totarget = (currenttarget - this.transform.position).normalized;
        if(Vector3.Dot(movedir, totarget) < 0f)
        {
            transform.position = currenttarget;
            state = MovingPlatformState.Paused;
        }
    }
}

public enum MovingPlatformState
{
    TowardsDestination,
    TowardsStart,
    Paused
}