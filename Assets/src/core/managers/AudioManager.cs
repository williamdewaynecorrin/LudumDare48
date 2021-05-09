using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance = null;

    [SerializeField]
    private AudioClip levelmusic;
    private AudioSource source;
    private int fadeframes = 180;
    private bool musicmatch = false;
    private float musictime = 0f;

    void Awake()
    {
        if (instance == null)
        {
            InitInstance();
        }
        else
        {
            if (instance.source.clip == this.levelmusic)
                instance.musicmatch = true;
            else
            {
                instance.source.clip = this.levelmusic;
                instance.musicmatch = false;
            }
            GameObject.Destroy(this.gameObject);
        }
    }
    void InitInstance()
    {
        instance = this;
        source = GetComponent<AudioSource>();
        source.clip = levelmusic;
        source.volume = 0f;
        source.loop = true;
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    // -- music fade
    public static void FadeIn()
    {
        instance.source.volume = 0f;
        if(!instance.musicmatch)
            instance.source.Play();

        instance.StartCoroutine(instance.FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        float val = 1f / (float)fadeframes;
        for(int i = 0; i < fadeframes; ++i)
        {
            source.volume += val;
            yield return new WaitForEndOfFrame();
        }

        source.volume = 1f;
        yield return null;
    }

    public static void FadeOut()
    {
        instance.source.volume = 1f;
        instance.StartCoroutine(instance.FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        float val = 1f / (float)fadeframes;
        for (int i = 0; i < fadeframes; ++i)
        {
            source.volume -= val;
            yield return new WaitForEndOfFrame();
        }

        source.volume = 0f;
        yield return null;
    }

    public static bool MusicMatch()
    {
        return instance.musicmatch;
    }

    public static void RecordMusicTime()
    {
        instance.musictime = instance.source.time;
    }

    public static void SetMusicTime()
    {
        instance.source.time = instance.musictime;
    }

    public static void ResetMusicTime()
    {
        instance.musictime = 0f;
        instance.source.time = instance.musictime;
    }

    public static void PlayStitchedAudio(AudioClip[] clips, float stitchtime)
    {
        instance.StartCoroutine(instance.StitchAudio(clips, stitchtime));
    }

    IEnumerator StitchAudio(AudioClip[] clips, float stitchtime)
    {
        for (int startidx = 0; startidx < clips.Length; ++startidx)
        {
            PlayClip2D(clips[startidx]);
            yield return new WaitForSeconds(clips[startidx].length + stitchtime);
        }

        yield return null;
    }

    public static void PlayDelayedAudio(AudioClip clip, float waittime)
    {
        instance.StartCoroutine(instance.DelayedAudio(clip, waittime));
    }

    IEnumerator DelayedAudio(AudioClip clip, float waittime)
    {
        yield return new WaitForSeconds(waittime);
        PlayClip2D(clip);

        yield return null;
    }

    // -- sfx play
    public static AudioSource PlayClip2D(AudioClip clip)
    {
        AudioSource audio = new GameObject("audio_" + clip.name).AddComponent<AudioSource>();
        audio.clip = clip;
        audio.spatialBlend = 0.0f;
        audio.Play();
        GameObject.Destroy(audio.gameObject, clip.length);

        return audio;
    }

    public static AudioSource PlayClip3D(AudioClip clip, Vector3 position)
    {
        AudioSource audio = new GameObject("audio3D_" + clip.name).AddComponent<AudioSource>();
        audio.transform.position = position;
        audio.clip = clip;
        audio.spatialBlend = 0.75f;
        audio.Play();
        GameObject.Destroy(audio.gameObject, clip.length);

        return audio;
    }

    public static AudioClip GetRandomClip(AudioClip[] clips)
    {
        int i = Random.Range(0, clips.Length);
        return clips[i];
    }

    public static AudioSource PlayRandomClip2D(AudioClip[] clips)
    {
        int i = Random.Range(0, clips.Length);
        return PlayClip2D(clips[i]);
    }

    public static AudioSource PlayRandomClip3D(AudioClip[] clips, Vector3 position)
    {
        int i = Random.Range(0, clips.Length);
        return PlayClip3D(clips[i], position);
    }
}

[System.Serializable]
public class AudioClipCollection
{
    public AudioClip[] sounds;
}