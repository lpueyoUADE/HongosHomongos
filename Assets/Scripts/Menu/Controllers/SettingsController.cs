using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsController : MenuesControllerBase
{
    public AudioMixer mixerSounds;

    [Header("Other menues")]
    public GameObject mainMenuObject;

    [Header("This menu buttons")]
    public Button back;
    
    [Header("Settings sliders")]
    public Slider soundsSlider;
    public Slider uiSlider;
    public Slider musicSlider;

    public override void ListenToEvents()
    {
        back.onClick.AddListener(Back);
        soundsSlider.onValueChanged.AddListener(delegate{SliderSounds();});
        uiSlider.onValueChanged.AddListener(delegate{SliderUI();});
        musicSlider.onValueChanged.AddListener(delegate{SliderMusic();});

        mixerSounds.GetFloat("Effects", out float soundsVolume);
        soundsSlider.SetValueWithoutNotify(Mathf.Pow(10f, soundsVolume / 20));

        mixerSounds.GetFloat("UI", out float uiVolume);
        uiSlider.SetValueWithoutNotify(Mathf.Pow(10f, uiVolume / 20));

        mixerSounds.GetFloat("Music", out float musicVolume);
        musicSlider.SetValueWithoutNotify(Mathf.Pow(10f, musicVolume / 20));
    }

    public override void StopListenToEvents()
    {
        back.onClick.RemoveListener(Back);
        soundsSlider.onValueChanged.RemoveListener(delegate{SliderSounds();});
        uiSlider.onValueChanged.RemoveListener(delegate{SliderUI();});
        musicSlider.onValueChanged.RemoveListener(delegate{SliderMusic();});
    }

    private void SliderSounds()
    {
        mixerSounds.SetFloat("Effects", Mathf.Log10(soundsSlider.value) * 20); 
    }

    private void SliderUI()
    {
        mixerSounds.SetFloat("UI", Mathf.Log10(uiSlider.value) * 20); 
    }

    private void SliderMusic()
    {
        mixerSounds.SetFloat("Music", Mathf.Log10(musicSlider.value) * 20); 
    }

    private void Back()
    {
        gameObject.SetActive(false);
        MainMenuEvents.OnPlayButtonSound?.Invoke();
        mainMenuObject.SetActive(true);
    }
}
