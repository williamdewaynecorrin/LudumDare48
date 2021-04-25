using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class MainMenuBike : MonoBehaviour
{
    [SerializeField]
    private BikeWheel frontwheel;
    [SerializeField]
    private BikeWheel backwheel;
    [SerializeField]
    private Vector3 velocity;
    [SerializeField]
    private float wheelspinrate = 300f;
    [SerializeField]
    private Transform loopbegin;

    private new Rigidbody rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        frontwheel.SetTorque(wheelspinrate);
        backwheel.SetTorque(wheelspinrate);

        rigidbody.velocity = velocity;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<LoopEnd>() != null)
            transform.position = loopbegin.position;
    }
}
