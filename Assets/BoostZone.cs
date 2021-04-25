using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostZone : MonoBehaviour
{
    public Transform directiontransform;

    void Awake()
    {

    }

    void Update()
    {

    }

    public Vector3 GetBoostDirection()
    {
        return directiontransform.forward;
    }
}
