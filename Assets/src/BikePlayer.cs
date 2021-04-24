using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikePlayer : MonoBehaviour
{
    //==================================================================================================================================================
    // -- Properties
    //==================================================================================================================================================
    [Header("Wheels")]
    [SerializeField]
    private WheelCollider frontwheel;
    [SerializeField]
    private WheelCollider backwheel;
    [SerializeField]
    private MeshRenderer frontwheelmesh;
    [SerializeField]
    private MeshRenderer backwheelmesh;

    [Header("Physics Values")]
    [SerializeField]
    private float torque = 200f;
    [SerializeField]
    private float acceleration = 1.0f;
    [SerializeField]
    private float stopfriction = 0.7f;
    [SerializeField] 
    private float maxspeed = 0.2f;
    [SerializeField]
    private float lateralmult = 0.075f;

    [SerializeField]
    private float rotationsmoothing = 10.0f;

    private new Rigidbody rigidbody;
    private Vector3 currentvelocity;
    private Vector3 lookdirection;
    private Vector2 framemovementinput;
    private bool grounded = false;
    private bool framehasinput = false;
    private Vector3 tirecolliderinitiallocalpos;

    //==================================================================================================================================================
    // -- Initialization methods
    //==================================================================================================================================================
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        lookdirection = transform.forward;
        tirecolliderinitiallocalpos = frontwheel.transform.localPosition;
    }

    void Start()
    {

    }

    //==================================================================================================================================================
    // -- Update / input methods
    //==================================================================================================================================================
    void Update()
    {
        framemovementinput = GetMovementInput();
        framehasinput = framemovementinput != Vector2.zero;

        if (framehasinput)
        {
            Quaternion targetrotation = Quaternion.LookRotation(lookdirection, Vector3.up);
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetrotation, rotationsmoothing * Time.deltaTime);
        }
    }

    private Vector2 GetMovementInput()
    {
        float xkeyboardin = Input.GetKey(KeyCode.D) ? 1.0f : (Input.GetKey(KeyCode.A) ? -1.0f : 0.0f);
        //float xcontrollerin = Input.GetAxis("Horizontal");
        float xin = Mathf.Clamp(xkeyboardin, -1f, 1f);

        float ykeyboardin = Input.GetKey(KeyCode.W) ? 1.0f : (Input.GetKey(KeyCode.S) ? -1.0f : 0.0f);
        //float ycontrollerin = Input.GetAxis("Vertical");
        float yin = Mathf.Clamp(ykeyboardin, -1f, 1f);

        return new Vector2(xin, yin);
    }

    //==================================================================================================================================================
    // -- Fixed update / physics methods
    //==================================================================================================================================================
    void FixedUpdate()
    {
        // -- movement logic
        Vector3 right = Vector3.right; // change to camera.right later
        Vector3 forward = Vector3.forward; // change to camera.forward later

        Vector3 framevelocity = CalculateWheelAccel(right * lateralmult, forward);
        if (framevelocity == Vector3.zero)
            currentvelocity *= stopfriction;
        else
        {
            currentvelocity += framevelocity;
            currentvelocity = Vector3.ClampMagnitude(currentvelocity, maxspeed);
        }

        //transform.position += currentvelocity;
        if (currentvelocity != Vector3.zero)
            lookdirection = currentvelocity.normalized;
    }

    private Vector3 CalculateWheelAccel(Vector3 right, Vector3 forward)
    {
        Vector3 framemovement = forward * framemovementinput.y;
        framemovement *= acceleration;

        // -- apply torque to all wheels
        float motortorque = framemovementinput.y * torque;
        frontwheel.motorTorque = motortorque;
        backwheel.motorTorque = motortorque;

        // -- apply pose to real wheels
        Vector3 offset = Vector3.right * -tirecolliderinitiallocalpos.x;
        Quaternion offsetr = Quaternion.Euler(0f, 90f, 0f);
        frontwheel.GetWorldPose(out Vector3 posfront, out Quaternion rotfront);
        frontwheelmesh.transform.position = posfront + offset;
        frontwheelmesh.transform.rotation = rotfront * offsetr;

        backwheel.GetWorldPose(out Vector3 posback, out Quaternion rotback);
        backwheelmesh.transform.position = posback + offset;
        backwheelmesh.transform.rotation = rotback * offsetr;

        return framemovement;
    }

    //==================================================================================================================================================
    // -- Debug methods
    //==================================================================================================================================================
    void OnGUI()
    {
        float debuglineheight = 26f;
        Rect baserect = new Rect(10f, 10f, 200f, 25f);

        GUI.TextField(baserect, string.Format("Movement Input: {0}", framemovementinput));
        baserect.y += debuglineheight;

        GUI.TextField(baserect, string.Format("Grounded?: {0}", grounded));
        baserect.y += debuglineheight;
    }
}
