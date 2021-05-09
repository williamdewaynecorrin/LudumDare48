using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventAction : MonoBehaviour
{
    [SerializeField]
    private Transform finalstate;
    private StoredTransform initialstate;

    [SerializeField]
    private bool useexplicitinitialstate = false;
    [ConditionalHide("useexplicitinitialstate", true)]
    [SerializeField]
    private Transform explicitinitialstate;

    [SerializeField]
    private float lerp = 5f;

    [SerializeField]
    private AudioClip activatenoise, deactivatenoise;

    private bool activated = false;

    // Start is called before the first frame update
    void Awake()
    {
        initialstate = new StoredTransform(this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if(activated)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, finalstate.position, lerp * Time.deltaTime);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, finalstate.rotation, lerp * Time.deltaTime);
            this.transform.localScale = Vector3.Lerp(this.transform.localScale, finalstate.localScale, lerp * Time.deltaTime);
        }
        else
        {
            if (explicitinitialstate == null)
            {
                this.transform.position = Vector3.Lerp(this.transform.position, initialstate.position, lerp * Time.deltaTime);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, initialstate.rotation, lerp * Time.deltaTime);
                this.transform.localScale = Vector3.Lerp(this.transform.localScale, initialstate.scale, lerp * Time.deltaTime);
            }
            else
            {
                this.transform.position = Vector3.Lerp(this.transform.position, explicitinitialstate.position, lerp * Time.deltaTime);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, explicitinitialstate.rotation, lerp * Time.deltaTime);
                this.transform.localScale = Vector3.Lerp(this.transform.localScale, explicitinitialstate.localScale, lerp * Time.deltaTime);
            }
        }
    }

    public void Activate(bool activate)
    {
        this.activated = activate;

        if (activated)
            AudioManager.PlayClip2D(activatenoise);
        else
            AudioManager.PlayClip2D(deactivatenoise);
    }
}
