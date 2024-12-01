using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestInGameUI : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioClip endGameMusic;
    public AudioSource musicSource;

    [Header("Pause menu stuff")]
    public GameObject pauseObject;
    public Button resumeGameButton;
    public Button exitGameButton;
    public Button resultsExitButton;
    public Slider soundsSlider;
    public Slider uiSlider;
    public Slider musicSlider;
    public AudioClip pauseSound;

    [Header("Utils")]
    public Sprite _deniedSprite;

    [Header("Timer")]
    public TextMeshProUGUI _turnTimeText;
    public Animator _turnTimeAnimation;
    public string _waitingText;

    [Header("Charging bar")]
    public Image _chargingWeaponBar;
    public float _chargingSpeed = 1;
    public float _chargeWeaponBarValue;     // Sets charging bar fill amount
    public bool _chargeWeaponBar;           // Show/Hide charging bar
    public float _chargeWeaponBarStartTime; // Makes charging bar reset when ping poing
    public bool _lockWeaponChargeBar;       // Makes ping pong work
    private static float _chargingBarPower; // Used for AI
    public static float CurrentChargeBarPower => _chargingBarPower;

    [Header("Result")]
    public GameObject _resultsObject;
    public TextMeshProUGUI _resultsText;

    [Header("Portraits")]
    [Header("Abilities portraits")]
    public HorizontalLayoutGroup _abilitiesPortraitsLayout;
    public List<InGameUIAbilityPortrait> _abilityPortraitsList = new();
    public List<float> _abilityChargingSpeeds = new();

    [Header("Characters Portraits")]
    public InGameUICharacterPortrait _portraitPrefab;
    public HorizontalLayoutGroup _portraitsLayout;
    public int _portraitOffset = 55;
    public List<InGameUICharacterPortrait> _portraitsList = new();
    public Dictionary<BaseCharacter, InGameUICharacterPortrait> _portraitsAndCharacters = new();

    [Header("Others")]
    public GameObject freeLookTextrObject;
    public int _latestSelectedAbility = 0;
    private AudioSource _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _chargingBarPower = 0;

        InGameUIEvents.OnUpdateTurnTime += UpdateTurnTime;
        InGameUIEvents.OnChargingWeaponBar += ChargingBar;
        InGameUIEvents.OnTimerWait += WaitTimer;
        InGameUIEvents.OnPlayUISound += PlayUISound;
        InGameUIEvents.OnAddCharacterPortrait += OnAddCharacterPortrait;
        InGameUIEvents.OnPortraitUpdate += PortraitUpdateAnim;
        InGameUIEvents.OnCharacterPortraitUpdate += PortraitUpdateAnim;
        InGameUIEvents.OnFreeLookMode += FreeLookModeText;
        InGameUIEvents.OnAbilityPortrait += AbilityPortraitUpdateList;
        InGameUIEvents.OnUpdateAbilityPortrait += AbilityPortraitClear;
        InGameUIEvents.OnAbilityPortraitSelected += AbilityPortraitSelected;

        GameTurnEvents.OnGameEnded += ShowResultText;
    }

    private void Start()
    {
        if (_portraitsLayout) _portraitsLayout.padding.left += _portraitOffset;
        _chargeWeaponBarStartTime = Time.time;
    }

    private void OnDestroy()
    {
        InGameUIEvents.OnUpdateTurnTime -= UpdateTurnTime;
        InGameUIEvents.OnChargingWeaponBar -= ChargingBar;
        InGameUIEvents.OnTimerWait -= WaitTimer;
        InGameUIEvents.OnPlayUISound -= PlayUISound;
        InGameUIEvents.OnAddCharacterPortrait -= OnAddCharacterPortrait;
        InGameUIEvents.OnPortraitUpdate -= PortraitUpdateAnim;
        InGameUIEvents.OnCharacterPortraitUpdate -= PortraitUpdateAnim;
        InGameUIEvents.OnFreeLookMode -= FreeLookModeText;
        InGameUIEvents.OnAbilityPortrait -= AbilityPortraitUpdateList;
        InGameUIEvents.OnUpdateAbilityPortrait -= AbilityPortraitClear;
        InGameUIEvents.OnAbilityPortraitSelected -= AbilityPortraitSelected;

        GameTurnEvents.OnGameEnded -= ShowResultText;

        resumeGameButton.onClick.RemoveListener(ResumeGameButton);
        resultsExitButton.onClick.RemoveListener(ExitResultsButton);
        soundsSlider.onValueChanged.RemoveListener(delegate{SliderSounds();});
        uiSlider.onValueChanged.RemoveListener(delegate{SliderUI();});
        musicSlider.onValueChanged.RemoveListener(delegate{SliderMusic();});
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel") && !GameTurnEvents.IsGameFinished)
        {
            if (pauseObject.activeSelf) GamePause(false);
            else GamePause(true);
        }

        if (_chargeWeaponBar)
        {
            if (!_lockWeaponChargeBar) _chargeWeaponBarStartTime = Time.time;

            _lockWeaponChargeBar = true;
            _chargeWeaponBarValue = (Time.time - _chargeWeaponBarStartTime) * _chargingSpeed;
            _chargingWeaponBar.fillAmount = Mathf.PingPong(_chargeWeaponBarValue, 1);

            float power = _chargingWeaponBar.fillAmount;
            _chargingBarPower = power;
            InGameUIEvents.UpdateChargeBarIntensity(power); // Fill amount is used to get values from 0 to 1.
        }
        
        else
        {
            _chargeWeaponBarValue = 0;
            _chargingWeaponBar.fillAmount = 0;
            _lockWeaponChargeBar = false;

            _chargingBarPower = 0;
            InGameUIEvents.UpdateChargeBarIntensity();
        }
    }

    private void GamePause(bool isPaused)
    {
        InGameUIEvents.OnPlayUISound(pauseSound);

        if (isPaused)
        {
            GameTurnEvents.UpdateGamePaused(true);
            pauseObject.SetActive(true);

            resumeGameButton.onClick.AddListener(ResumeGameButton);
            exitGameButton.onClick.AddListener(ExitGameButton);
            soundsSlider.onValueChanged.AddListener(delegate{SliderSounds();});
            uiSlider.onValueChanged.AddListener(delegate{SliderUI();});
            musicSlider.onValueChanged.AddListener(delegate{SliderMusic();});

            Time.timeScale = 0;
        }

        else
        {
            resumeGameButton.onClick.RemoveListener(ResumeGameButton);
            exitGameButton.onClick.RemoveListener(ExitGameButton);
            soundsSlider.onValueChanged.RemoveListener(delegate{SliderSounds();});
            uiSlider.onValueChanged.RemoveListener(delegate{SliderUI();});
            musicSlider.onValueChanged.RemoveListener(delegate{SliderMusic();});

            pauseObject.SetActive(false);
            Time.timeScale = 1;
            GameTurnEvents.UpdateGamePaused(false);
        }
    }

    private void WaitTimer()
    {
        _turnTimeText.text = _waitingText;
        _turnTimeAnimation.SetTrigger("TimeWait");
    }

    private void UpdateTurnTime(string newTime, bool isNumeric = true)
    {
        _turnTimeText.text = newTime;

        if (isNumeric)
        {
            if (newTime == "0")
            {
                _audio.PlayOneShot(GameManagerEvents.ModeSettings.ClockEndTurnSound);
                _turnTimeAnimation.SetTrigger("TimeOut");
            }

            else
            {
                _audio.PlayOneShot(GameManagerEvents.ModeSettings.ClockTickSound);
                _turnTimeAnimation.SetTrigger("TimePassing");
            }
        }
    }

    private void ChargingBar(bool show)
    {
        _chargeWeaponBar = show;
        _chargingWeaponBar.gameObject.SetActive(show);
    }

    private void ShowResultText()
    {
        resultsExitButton.onClick.AddListener(ExitResultsButton);
        musicSource.Stop();
        musicSource.clip = endGameMusic;
        musicSource.Play();

        _chargingWeaponBar.gameObject.SetActive(false);
        _turnTimeText.gameObject.SetActive(false);
        _abilitiesPortraitsLayout.gameObject.SetActive(false);
        _portraitsLayout.gameObject.SetActive(false);

        _resultsObject.SetActive(true);
        _resultsText.text = GameManager._playerAliveCharacters.Count == 0 ? "YOU LOST!" : "YOU WIN!";
    }

    private void PlayUISound(AudioClip clip)
    {
        _audio.PlayOneShot(clip);
    }

    private void AbilityPortraitUpdateList(List<CharacterAbilityData> newList)
    {
        _abilitiesPortraitsLayout.gameObject.SetActive(true);
        _abilityChargingSpeeds.Clear();
        int index = 1;
        foreach (CharacterAbilityData item in newList)
        {
            _abilityPortraitsList[index - 1].UpdatePortrait(item.AbilityPortraitSprite, index, item.AbilityName);
            _abilityChargingSpeeds.Add(item.AbilitProjectileChargingSpeed);
            index++;
        }

        switch (index)
        {
            case 2:
                _abilityPortraitsList[1].UpdatePortrait(_deniedSprite, 2, "");
                _abilityPortraitsList[2].UpdatePortrait(_deniedSprite, 3, "");
                break;
            case 3:
                _abilityPortraitsList[2].UpdatePortrait(_deniedSprite, 3, "");
                break;
        }
    }

    private void AbilityPortraitClear(int index, bool clearAll = false)
    {
        if (clearAll || index >= 3)
        {
            foreach (InGameUIAbilityPortrait item in _abilityPortraitsList) item.UpdatePortrait(_deniedSprite);
            AbilityPortraitSelected(index);
            _abilitiesPortraitsLayout.gameObject.SetActive(false);
        }
        else _abilityPortraitsList[index].UpdatePortrait(_deniedSprite);
    }

    private void AbilityPortraitSelected(int newSelection)
    {
        _latestSelectedAbility = newSelection;
        if (newSelection <= 2) _chargingSpeed = _abilityChargingSpeeds[newSelection];
        else _chargingSpeed = 0;

        switch (newSelection)
        {
            case 0:
                _abilityPortraitsList[1].UpdateAnim(AbilityStatus.Idle);
                _abilityPortraitsList[2].UpdateAnim(AbilityStatus.Idle);
                _abilityPortraitsList[0].UpdateAnim(AbilityStatus.Selected);
            break;

            case 1:
                _abilityPortraitsList[0].UpdateAnim(AbilityStatus.Idle);
                _abilityPortraitsList[2].UpdateAnim(AbilityStatus.Idle);
                _abilityPortraitsList[1].UpdateAnim(AbilityStatus.Selected);
            break;

            case 2:
                _abilityPortraitsList[0].UpdateAnim(AbilityStatus.Idle);
                _abilityPortraitsList[1].UpdateAnim(AbilityStatus.Idle);
                _abilityPortraitsList[2].UpdateAnim(AbilityStatus.Selected);
            break;

            case 3: // Reset all to idle
                _abilityPortraitsList[0].UpdateAnim(AbilityStatus.Idle);
                _abilityPortraitsList[1].UpdateAnim(AbilityStatus.Idle);
                _abilityPortraitsList[2].UpdateAnim(AbilityStatus.Idle);
            break;

            case 4: // Reset all to idle
                _abilityPortraitsList[0].UpdateAnim(AbilityStatus.Disabled);
                _abilityPortraitsList[1].UpdateAnim(AbilityStatus.Disabled);
                _abilityPortraitsList[2].UpdateAnim(AbilityStatus.Disabled);
            break;
        }
    }

    private void OnAddCharacterPortrait(BaseCharacter characterReference)
    {
        InGameUICharacterPortrait portrait = Instantiate(_portraitPrefab);
        portrait.UpdateCharacterPortrait(characterReference.CharacterData.Portrait);

        portrait.transform.SetParent(_portraitsLayout.transform);
        _portraitsLayout.padding.left -= _portraitOffset;
        _portraitsList.Add(portrait);

        _portraitsAndCharacters.Add(characterReference, portrait);
    }

    private void PortraitUpdateAnim(BaseCharacter character, PortraitStatus newStatus = PortraitStatus.Idle)
    {
        if (!_portraitsAndCharacters.ContainsKey(character)) return;
        PortraitUpdateAnim(_portraitsList.IndexOf(_portraitsAndCharacters[character]), newStatus);
    }

    private void PortraitUpdateAnim(int index, PortraitStatus newStatus = PortraitStatus.Idle)
    {
        _portraitsList[index].UpdatePortraitAnimation(newStatus);
    }

    private void FreeLookModeText(bool enabled)
    {
        freeLookTextrObject.SetActive(enabled);
    }

    // Settings & Buttons
    private void SliderSounds()
    {
        audioMixer.SetFloat("Effects", Mathf.Log10(soundsSlider.value) * 20); 
    }

    private void SliderUI()
    {
        audioMixer.SetFloat("UI", Mathf.Log10(uiSlider.value) * 20); 
    }

    private void SliderMusic()
    {
        audioMixer.SetFloat("Music", Mathf.Log10(musicSlider.value) * 20); 
    }

    private void ResumeGameButton()
    {
        GamePause(false);
    }

    private void ExitGameButton()
    {
        GamePause(false);
        StartCoroutine(GotoLevel("MainMenu"));
    }

    private void ExitResultsButton()
    {
        resultsExitButton.onClick.RemoveListener(ExitResultsButton);
        StartCoroutine(GotoLevel("MainMenu"));
    }

    IEnumerator GotoLevel(string scene)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}