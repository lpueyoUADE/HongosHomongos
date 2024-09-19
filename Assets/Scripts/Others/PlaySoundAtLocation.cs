using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundAtLocation : MonoBehaviour
{
    public AudioSource _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        PlaySoundEvents.PlaySound += Play;
    }

    private void OnDestroy()
    {
        PlaySoundEvents.PlaySound -= Play;
    }

    public void Play(Vector3 location, AudioClip clip, float volume = 1)
    {
        transform.position = location;
        _audio.PlayOneShot(clip, volume);
    }
}
