using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutscenePlayer : MonoBehaviour
{
    private static int blinkframetime = 3;

    [Header("Basic Properties")]
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float movespeed = 0.07f;
    [SerializeField]
    private bool useglobalcustomizationdata;

    [Header("Facial Animation")]
    [SerializeField]
    private MeshRenderer eyelrenderer;
    [SerializeField]
    private MeshRenderer eyerrenderer;
    [SerializeField]
    private MeshRenderer mouthrenderer;
    [SerializeField]
    private Texture2D eyeblinktexture_albedo;
    [SerializeField]
    private Texture2D eyeblinktexture_emission;
    [SerializeField]
    private Texture2D mouthopentexture;
    [SerializeField]
    private Texture2D mouthtalkingtexture;
    [SerializeField]
    private Timer talktimer;
    [SerializeField]
    private Timer blinktimer;

    [Header("Talking Sounds")]
    [SerializeField]
    private AudioClip[] talkingsounds;
    [SerializeField]
    private Texture2D uipicture;

    //private PlayerCustomization customization;
    private Texture2D[] eyeblinktextures;

    private Texture2D[] eyestandardtextures;
    private Texture2D mouthstandardtexture;

    private bool talking = false;
    private TalkState talkstate = TalkState.MouthClosed;
    private bool blinking = false;
    private BlinkState blinkstate = BlinkState.EyeOpened;

    // Start is called before the first frame update
    void Awake()
    {
        talktimer.Init();
        blinktimer.Init();

        //customization = GetComponent<PlayerCustomization>();
        eyeblinktextures = new Texture2D[2] { eyeblinktexture_albedo, eyeblinktexture_emission };
    }

    void Start()
    {
        //if (useglobalcustomizationdata && customization.HasGlobalPrefs())
            //customization.ApplySavedPrefs();

        //eyestandardtextures = customization.GetEyeTexture();
        //mouthstandardtexture = customization.GetMouthTexture();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(talking)
        {
            talktimer.Decrement();
            if(talktimer.TimerReached())
            {
                talktimer.Reset();
                CycleTalkState();
            }
        }
        if (blinking)
        {
            blinktimer.Decrement();
            if(blinktimer.TimerReached())
            {
                CycleBlinkState();
            }
        }
    }

    private void CycleTalkState()
    {
        if (talkstate == TalkState.MouthOpened)
        {
            //customization.SetMouthTexture(mouthstandardtexture);
            talkstate = TalkState.MouthClosed;
        }
        else if (talkstate == TalkState.MouthClosed)
        {
            //customization.SetMouthTexture(mouthtalkingtexture);
            talkstate = TalkState.MouthOpened;
        }
    }

    private void CycleBlinkState()
    {
        if (blinkstate == BlinkState.EyeOpened)
        {
            //customization.SetEyeTexture(eyeblinktextures);
            blinkstate = BlinkState.EyeClosed;
            blinktimer.frametime = blinkframetime;
        }
        else if (blinkstate == BlinkState.EyeClosed)
        {
            //customization.SetEyeTexture(eyestandardtextures);
            blinkstate = BlinkState.EyeOpened;
            blinktimer.Reset();
        }
    }

    public void OpenEyes()
    {
        //customization.SetEyeTexture(eyestandardtextures);
    }

    public void CloseEyes()
    {
        //customization.SetEyeTexture(eyeblinktextures);
    }

    public void OpenMouth()
    {
        //customization.SetMouthTexture(mouthopentexture);
    }

    public void CloseMouth()
    {
        //customization.SetMouthTexture(mouthstandardtexture);
    }

    public void StartTalking()
    {
        talking = true;
        talktimer.Reset();
        //customization.SetMouthTexture(mouthtalkingtexture);
        talkstate = TalkState.MouthOpened;
    }

    public void StopTalking()
    {
        talking = false;
        talktimer.Reset();
        //customization.SetMouthTexture(mouthstandardtexture);
        talkstate = TalkState.MouthClosed;
    }

    public void StartBlinking()
    {
        blinking = true;
        blinktimer.frametime = blinkframetime;
        //customization.SetEyeTexture(eyeblinktextures);
        blinkstate = BlinkState.EyeClosed;
    }

    public void StopBlinking()
    {
        blinking = false;
        blinktimer.Reset();
        //customization.SetEyeTexture(eyestandardtextures);
        blinkstate = BlinkState.EyeOpened;
    }

    public AudioClip[] TalkingSounds()
    {
        return talkingsounds;
    }

    public Texture2D GetUIPicture()
    {
        return uipicture;
    }
}

public enum BlinkState
{
    EyeOpened,
    EyeClosed
}

public enum TalkState
{
    MouthOpened,
    MouthClosed
}