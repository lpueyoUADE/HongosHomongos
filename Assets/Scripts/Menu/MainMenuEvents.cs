using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuEvents : MonoBehaviour
{
    public List<SceneData> scenarios = new();

    public GameObject mainMenuObject;
    private AudioSource _audio;
    public AudioClip clickSound;

    public static Action OnPlayButtonSound;

    private void Awake() 
    {
        _audio = GetComponent<AudioSource>();
        OnPlayButtonSound += PlayButtonSound;
    }

    private void OnDestroy() 
    {
        OnPlayButtonSound -= PlayButtonSound;
    }

    public void PlayButtonSound()
    {
        _audio.PlayOneShot(clickSound);
    }
}
