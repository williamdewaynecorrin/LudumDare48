using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip levelmusic;
    [SerializeField]
    private float musicvolume = 0.7f;
    [SerializeField]
    private float musicvolumedelta = 0.01f;

    private new AudioSource audio;
    private static MusicManager instance = null;
    private ESmartAudioState state = ESmartAudioState.Stopped;

    void Awake()
    {
        if(instance == null)
        {
            audio = GetComponent<AudioSource>();
            instance = this;
            audio.loop = true;
            audio.spatialBlend = 0f;

            audio.clip = levelmusic;
            audio.volume = 0f;
            FadeIn();

            GameObject.DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            instance.audio.clip = this.levelmusic;
            instance.audio.volume = 0f;
            FadeIn();

            GameObject.Destroy(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        if(state == ESmartAudioState.Starting)
        {
            audio.volume += musicvolumedelta;
            if(audio.volume >= musicvolume)
            {
                audio.volume = musicvolume;
                state = ESmartAudioState.Started;
            }
        }
        else if(state == ESmartAudioState.Stopping)
        {
            audio.volume -= musicvolumedelta;
            if (audio.volume <= 0f)
            {
                audio.volume = 0f;
                state = ESmartAudioState.Stopped;
                audio.Stop();
            }
        }
    }

    public static void FadeIn()
    {
        instance.audio.Play();
        instance.state = ESmartAudioState.Starting;
    }

    public static void FadeOut()
    {
        instance.state = ESmartAudioState.Stopping;
    }

    public static ESmartAudioState GetState()
    {
        return instance.state;
    }
}

