using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlatformerPlayer : MonoBehaviour
{
    //==================================================================================================================================================
    // -- Properties
    //==================================================================================================================================================
    [SerializeField]
    private float acceleration = 0.01f, stopfriction = 0.7f, maxspeed = 0.2f;
    [SerializeField]
    private float spherecastdist = 0.1f;
    [SerializeField]
    private new CapsuleCollider collider;
    [SerializeField]
    private LayerMask groundmask;
    [SerializeField]
    private float jumpstrength = 3.0f;

    [SerializeField]
    private float rotationsmoothing = 10.0f;

    private new Rigidbody rigidbody;
    private Vector3 currentvelocity;
    private Vector3 lookdirection;
    private Vector2 framemovementinput;
    private bool grounded = false;
    private bool framehasinput = false;

    //==================================================================================================================================================
    // -- Initialization methods
    //==================================================================================================================================================
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        lookdirection = transform.forward;
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
            transform.rotation = Quaternion.Slerp(transform.rotation, targetrotation, rotationsmoothing * Time.deltaTime);
        }
    }

    private Vector2 GetMovementInput()
    {
        float xkeyboardin = Input.GetKey(KeyCode.D) ? 1.0f : (Input.GetKey(KeyCode.A) ? -1.0f : 0.0f);
        //float xcontrollerin = Input.GetAxis("Horizontal");
        float xin = Mathf.Clamp(xkeyboardin , -1f, 1f);

        float ykeyboardin = Input.GetKey(KeyCode.W) ? 1.0f : (Input.GetKey(KeyCode.S) ? -1.0f : 0.0f);
        //float ycontrollerin = Input.GetAxis("Vertical");
        float yin = Mathf.Clamp(ykeyboardin , -1f, 1f);

        return new Vector2(xin, yin);
    }

    //==================================================================================================================================================
    // -- Fixed update / physics methods
    //==================================================================================================================================================
    void FixedUpdate()
    {
        //Vector3 framegravity = Vector3.up * rigidbody.velocity.y;
        //rigidbody.velocity = framegravity;

        // -- check for grounded
        Vector3 direction = Vector3.down;
        float halfheight = collider.height * 0.5f;
        Vector3 pointa = transform.position + collider.center + Vector3.up * -collider.height * 0.5f;
        Vector3 pointb = transform.position + collider.center + Vector3.up * collider.height;
        if (Physics.CapsuleCast(pointa, pointb, collider.radius, direction, out RaycastHit groundhit, spherecastdist, groundmask, QueryTriggerInteraction.Ignore))
        {
            grounded = true;

            // -- do angle test later
            float angle = Mathf.Abs(Vector3.Angle(groundhit.normal, Vector3.up));
        }
        else 
            grounded = false;

        if(grounded)
        {
            rigidbody.velocity = Vector3.zero;

            // -- movement logic
            Vector3 right = Vector3.right; // change to camera.right later
            Vector3 forward = Vector3.forward; // change to camera.forward later

            Vector3 framevelocity = CalculateFrameVelocity(right, forward);
            if (framevelocity == Vector3.zero)
                currentvelocity *= stopfriction;
            else
            {
                currentvelocity += framevelocity;
                currentvelocity = Vector3.ClampMagnitude(currentvelocity, maxspeed);
            }
        }
        else
        {
            // -- gravity logic
            rigidbody.velocity += PhysicsManager.gravity;
        }

        transform.position += currentvelocity;
        if (currentvelocity != Vector3.zero)
            lookdirection = currentvelocity.normalized;
    }

    private Vector3 CalculateFrameVelocity(Vector3 right, Vector3 forward)
    {
        Vector3 framemovement = right * framemovementinput.x + forward * framemovementinput.y;
        framemovement *= acceleration;

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
