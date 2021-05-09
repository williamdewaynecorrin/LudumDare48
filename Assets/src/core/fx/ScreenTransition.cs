using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransition : MonoBehaviour
{
    [SerializeField]
    private Material transition;
    [SerializeField]
    private string transitiontexturename = "_TransitionTex";
    [SerializeField]
    private string transitioncutoffname = "_Cutoff";
    [SerializeField]
    private Texture portaltransition;
    [SerializeField]
    private float transitionspeed = 12.5f;

    private float currentcutoff = 0.0f;
    private TRANSITIONSTATE state = TRANSITIONSTATE.Off;
    private TRANSITIONTYPE type = TRANSITIONTYPE.Portal;

    void Awake()
    {
        currentcutoff = 0f;
        state = TRANSITIONSTATE.Off;
        transition.SetTexture(transitiontexturename, portaltransition);
        transition.SetFloat(transitioncutoffname, currentcutoff);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, transition);
    }

    private void Update()
    {
        switch(state)
        {
            case TRANSITIONSTATE.Off:
                break;
            case TRANSITIONSTATE.TOn:
                currentcutoff += transitionspeed * Time.deltaTime;
                if (currentcutoff > 0.99f)
                {
                    currentcutoff = 1.0f;
                    state = TRANSITIONSTATE.On;
                    StateAchieved(state);
                }
                break;
            case TRANSITIONSTATE.On:
                break;
            case TRANSITIONSTATE.TOff:
                currentcutoff -= transitionspeed * Time.deltaTime;
                if (currentcutoff < 0.01f)
                {
                    currentcutoff = 0.0f;
                    state = TRANSITIONSTATE.Off;
                    StateAchieved(state);
                }
                break;
        }

        transition.SetFloat(transitioncutoffname, currentcutoff);
    }

    public void BeginFadeIn(TRANSITIONTYPE type)
    {
        this.type = type;
        if (type == TRANSITIONTYPE.Portal)
            transition.SetTexture(transitiontexturename, portaltransition);
        else if (type == TRANSITIONTYPE.Cutscene)
            transition.SetTexture(transitiontexturename, portaltransition);

        state = TRANSITIONSTATE.TOn;
        currentcutoff = 0.0f;
    }

    public void BeginFadeOut(TRANSITIONTYPE type)
    {
        this.type = type;
        if (type == TRANSITIONTYPE.Portal)
            transition.SetTexture(transitiontexturename, portaltransition);
        else if (type == TRANSITIONTYPE.Cutscene)
            transition.SetTexture(transitiontexturename, portaltransition);

        state = TRANSITIONSTATE.TOff;
        currentcutoff = 1.0f;
    }

    public void SetStateExplicit(TRANSITIONSTATE state)
    {
        this.state = state;
        if (state == TRANSITIONSTATE.On)
        {
            currentcutoff = 1.0f;
        }
        else if (state == TRANSITIONSTATE.Off)
        {
            currentcutoff = 0.0f;
        }
        transition.SetFloat(transitioncutoffname, currentcutoff);
    }

    // -- notify things that want to know about the transition finishing
    public void StateAchieved(TRANSITIONSTATE newstate)
    {
        if(newstate == TRANSITIONSTATE.On)
        {
            //GameManager.ExecuteLevelLoad(type);
        }
        else if (newstate == TRANSITIONSTATE.Off)
        {
            //GameManager.ExecuteLevelLoaded(type);
        }
    }
}

public enum TRANSITIONSTATE
{
    Off,
    TOn,
    On,
    TOff
}

public enum TRANSITIONTYPE
{
    Portal,
    Cutscene
}