using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeWheel : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer meshrenderer;
    [SerializeField]
    private float torquesmoothing = 10.0f;

    private float currenttorque = 0f;
    private float desiredtorque = 0f;
    private Quaternion desiredrot;
    private Quaternion baserotation;

    void Awake()
    {
        baserotation = transform.localRotation;
        desiredrot = Quaternion.Euler(0f, 0f, 0f);
    }

    void Update()
    {
        currenttorque = Mathf.Lerp(currenttorque, desiredtorque, Time.deltaTime * torquesmoothing);
        desiredrot *= Quaternion.Euler(0f, 0f, currenttorque);

        transform.localRotation = baserotation * desiredrot;
    }

    public void SetTorque(float torque)
    {
        desiredtorque = torque;
    }
}
