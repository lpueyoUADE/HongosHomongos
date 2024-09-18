using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestInGameUI : MonoBehaviour
{
    private AudioSource _audio;

    public TextMeshProUGUI _turnTimeText;
    public AudioClip _timeClickSound;
    public AudioClip _timeEndSound;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        InGameUIEvents.OnUpdateTurnTime += UpdateTurnTime;
    }

    private void OnDestroy()
    {
        InGameUIEvents.OnUpdateTurnTime -= UpdateTurnTime;
    }

    void UpdateTurnTime(string newTime)
    {
        _turnTimeText.text = newTime;

        if (newTime == "0")
        {
            _audio.PlayOneShot(_timeEndSound);
            _turnTimeText.color = Color.red;
        }

        else
        {
            _audio.PlayOneShot(_timeClickSound);
            _turnTimeText.color = Color.green;
        }
    }
}
