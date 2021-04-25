using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SmartAudio : MonoBehaviour
{
    private new AudioSource audio;
    private ESmartAudioState state = ESmartAudioState.Stopped;
    private float targetvolume = 1.0f;
    private float volumestep = 0.01f;

    void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        switch(state)
        {
            case ESmartAudioState.Starting:
                audio.volume += volumestep;
                if(audio.volume >= targetvolume)
                {
                    audio.volume = targetvolume;
                    state = ESmartAudioState.Started;
                }

                break;
            case ESmartAudioState.Started:

                // -- check for finished
                if (audio.time == audio.clip.length && !audio.loop)
                    state = ESmartAudioState.Finished;

                break;
            case ESmartAudioState.Stopping:
                audio.volume -= volumestep;
                if (audio.volume <= 0f)
                {
                    audio.volume = 0f;
                    state = ESmartAudioState.Stopped;
                    audio.Stop();
                }

                break;
            case ESmartAudioState.Stopped:
                break;
            case ESmartAudioState.Finished:
                break;
        }
    }

    public void SetClip(AudioClip clip)
    {
        audio.clip = clip;
    }

    public AudioClip Clip()
    {
        return audio.clip;
    }
        
    public void StopImmediate()
    {
        state = ESmartAudioState.Stopped;
        audio.volume = 0f;
        audio.Stop();
    }

    public void BeginStop(int frames)
    {
        state = ESmartAudioState.Stopping;
    }

    public void StartImmediate(AudioClip clip, bool looping)
    {
        state = ESmartAudioState.Started;
        audio.volume = targetvolume;
        audio.clip = clip;
        audio.loop = looping;
        audio.Play();
    }

    public void BeginStart(AudioClip clip, bool looping, int frames)
    {
        state = ESmartAudioState.Starting;
        audio.clip = clip;
        audio.loop = looping;
        audio.Play();
    }

    public ESmartAudioState State()
    {
        return state;
    }
}

public enum ESmartAudioState
{
    Stopped = 0,
    Starting = 1,
    Started = 2,
    Stopping = 3,
    Finished = 4
}