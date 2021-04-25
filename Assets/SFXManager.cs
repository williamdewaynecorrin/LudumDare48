using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
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
