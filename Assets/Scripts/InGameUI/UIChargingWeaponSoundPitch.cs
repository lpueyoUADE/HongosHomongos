using UnityEngine;
using UnityEngine.UI;

public class UIChargingWeaponSoundPitch : MonoBehaviour
{
    private AudioSource _audio;
    private Image _fillbar;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _fillbar = GetComponent<Image>();
        _audio.pitch = _fillbar.fillAmount;
    }

    private void Update()
    {
        _audio.pitch = _fillbar.fillAmount;
    }
}
