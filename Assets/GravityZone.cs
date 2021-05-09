using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityZone : MonoBehaviour
{
    public Transform directiontransform;
    public float gravityforce = 0.5f;

    void Awake()
    {

    }

    void Update()
    {
        
    }

    public float GravityForce()
    {
        return gravityforce;
    }

    public Vector3 GravityDirection()
    {
        return directiontransform.forward.normalized;
    }
}
