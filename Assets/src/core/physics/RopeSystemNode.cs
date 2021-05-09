using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSystemNode : MonoBehaviour
{
    [SerializeField]
    private RopeSystem system;

    private Rigidbody body;
    private RopeSystemNode uplink;
    private RopeSystemNode downlink;
    private int nodeidx = -1;
    private bool touchingplayer = false;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    void Start()
    {
        
    }

    public void SetupNode(int index, RopeSystemNode uplink, RopeSystemNode downlink)
    {
        nodeidx = index;
        this.uplink = uplink;
        this.downlink = downlink;
    }

    void Update()
    {
        
    }

    public bool TouchingPlayer()
    {
        return touchingplayer;
    }

    public float Radius()
    {
        return transform.localPosition.x;
    }

    public RopeSystemNode UpLink()
    {
        return uplink;
    }

    public RopeSystemNode DownLink()
    {
        return downlink;
    }

    public Vector3 GetPosition()
    {
        return transform.position - transform.up * transform.localScale.y * 2.5f;
    }

    public Vector3 UpLinkPos()
    {
        return uplink != null ? uplink.GetPosition() : GetPosition() + transform.up * transform.localScale.y;
    }

    public Vector3 DownLinkPos()
    {
        return downlink != null ? downlink.GetPosition() : GetPosition() - transform.up * transform.localScale.y;
    }

    public Vector3 GetClimbDirUp()
    {
        return uplink == null ? transform.up :
               (uplink.transform.position - transform.position).normalized;
    }

    public Vector3 GetClimbDirDown()
    {
        return uplink == null ? -transform.up :
               (downlink.transform.position - transform.position).normalized;
    }

    public int NodeIndex()
    {
        return nodeidx;
    }

    public void AddForceAtPosition(Vector3 pos, Vector3 force)
    {
        system.AddForceAtPosition(pos, force);
        body.AddForce(force);
    }

    void OnTriggerEnter(Collider collision)
    {
        //Player p = collision.gameObject.GetComponent<Player>();
        //if(p != null && p.CanGrabRope())
        //{
        //    p.GrabRope(system, this);
        //    p.SetCurrentRopeNode(this);
        //    touchingplayer = true;
        //}
    }

    void OnTriggerExit(Collider collision)
    {
        //Player p = collision.gameObject.GetComponent<Player>();
        //if (p != null)
        //{
        //    touchingplayer = false;
        //    //if (!system.AnyNodeTouchingPlayer())
        //    //    p.ExitRope();
        //}
    }
}
