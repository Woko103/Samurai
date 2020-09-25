using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public float soundToPlay = -1.0f; //-1 means don't play any sound
    public AudioClip[] audioClip;

    AudioSource audi;

    void Start ()
    {
        audi = GetComponent<AudioSource> ();
    }

    void Update ()
    {
        if (soundToPlay > -1.0f)
        {
            PlaySound((int) soundToPlay, 1);
            soundToPlay = -1.0f;
        }
    }

    void PlaySound(int clip, float volumeScale)
    {
        audi.PlayOneShot(audioClip [clip], volumeScale);
    }
}
