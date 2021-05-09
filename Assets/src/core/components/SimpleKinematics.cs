using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleKinematics : MonoBehaviour
{
    [SerializeField]
    private Vector3 initialvelocity;
    [SerializeField]
    private Vector3 initialacceleration;

    private Vector3 velocity;
    private Vector3 acceleration;

    // Start is called before the first frame update
    void Awake()
    {
        velocity = initialacceleration;
        acceleration = initialacceleration;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        velocity += acceleration;
        transform.position += velocity;
    }

    public void SetVelocity(Vector3 v)
    {
        velocity = v;
    }

    public void SetAcceleration(Vector3 a)
    {
        acceleration = a;
    }
}
