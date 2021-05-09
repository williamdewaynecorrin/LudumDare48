using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonEvent : MonoBehaviour
{
    [SerializeField]
    private bool oneshot = true;
    [SerializeField]
    private UnityEvent onpress;
    [SerializeField]
    private UnityEvent onrelease;

    [SerializeField]
    private Transform button;
    private StoredTransform buttonreleasedstate;
    [SerializeField]
    private Transform buttonpressedstate;
    [SerializeField]
    private AudioClip presssound;
    [SerializeField]
    private AudioClip releasesound;

    private float lerpspeed = 5f;
    private bool pressed = false;

    // Start is called before the first frame update
    void Awake()
    {
        buttonreleasedstate = new StoredTransform(button);
    }

    void Update()
    {
        if(pressed)
        {
            button.position = Vector3.Lerp(button.position, buttonpressedstate.position, Time.deltaTime * lerpspeed);
        }
        else
        {
            button.position = Vector3.Lerp(button.position, buttonreleasedstate.position, Time.deltaTime * lerpspeed);
        }
    }

    public void PressButton()
    {
        if (oneshot)
        {
            if (pressed)
                return;

            pressed = true;
            AudioManager.PlayClip2D(presssound);
            onpress.Invoke();
        }
        else
        {
            pressed = !pressed;

            if (pressed)
            {
                AudioManager.PlayClip2D(presssound);
                onpress.Invoke();
            }
            else
            {
                AudioManager.PlayClip2D(releasesound);
                onrelease.Invoke();
            }
        }
    }
}
