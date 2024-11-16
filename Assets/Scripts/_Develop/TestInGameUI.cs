using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestInGameUI : MonoBehaviour
{
    private AudioSource _audio;

    [Header("Timer")]
    public TextMeshProUGUI _turnTimeText;
    public Animator _turnTimeAnimation;
    public string _waitingText;

    [Header("Charging bar")]
    public Image _chargingWeaponBar;
    public float _chargeWeaponBarValue;     // Sets charging bar fill amount
    public bool _chargeWeaponBar;           // Show/Hide charging bar
    public float _chargeWeaponBarStartTime; // Makes charging bar reset when ping poing
    public bool _lockWeaponChargeBar;       // Makes ping pong work
    private static float _chargingBarPower; // Used for AI

    [Header("Result")]
    public TextMeshProUGUI _resultText;
    public static float CurrentChargeBarPower => _chargingBarPower;

    [Header("Portraits")]
    public HorizontalLayoutGroup _portraitsLayout;
    public int _portraitOffset = 55;
    public List<Image> _portraitsList = new();
    public Dictionary<BaseCharacter, Image> _portraitsAndCharacters = new();

    [Header("Others")]
    public GameObject freeLookTextrObject;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _chargingBarPower = 0;
        _resultText.text = "";

        InGameUIEvents.OnUpdateTurnTime += UpdateTurnTime;
        InGameUIEvents.OnChargingWeaponBar += ChargingBar;
        InGameUIEvents.OnTimerWait += WaitTimer;
        InGameUIEvents.OnPlayUISound += PlayUISound;
        InGameUIEvents.OnAddCharacterPortrait += OnAddCharacterPortrait;
        InGameUIEvents.OnPortraitUpdate += PortraitUpdateAnim;
        InGameUIEvents.OnFreeLookMode += FreeLookModeText;

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
        InGameUIEvents.OnFreeLookMode -= FreeLookModeText;

        GameTurnEvents.OnGameEnded -= ShowResultText;
    }

    private void Update()
    {
        if (_chargeWeaponBar)
        {
            if (!_lockWeaponChargeBar) _chargeWeaponBarStartTime = Time.time;

            _lockWeaponChargeBar = true;
            _chargeWeaponBarValue = (Time.time - _chargeWeaponBarStartTime) * 2;
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
        _resultText.text = GameManager._playerAliveCharacters.Count == 0 ? "YOU LOST!" : "YOU WIN!";
    }

    private void PlayUISound(AudioClip clip)
    {
        _audio.PlayOneShot(clip);
    }

    private void OnAddCharacterPortrait(BaseCharacter newPortrait)
    {
        GameObject portrait = Instantiate(newPortrait.CharacterData.Portrait);
        portrait.transform.SetParent(_portraitsLayout.transform);
        _portraitsLayout.padding.left -= _portraitOffset;
        _portraitsList.Add(portrait.GetComponent<Image>());

        _portraitsAndCharacters.Add(newPortrait, portrait.GetComponent<Image>());
    }

    private void PortraitUpdateAnim(BaseCharacter character, PortraitStatus newStatus = PortraitStatus.Idle)
    {

    }

    private void PortraitUpdateAnim(int index, PortraitStatus newStatus = PortraitStatus.Idle)
    {
        Animator portrait = _portraitsList[index].GetComponent<Animator>();

        if (portrait)
        {
            switch (newStatus)
            {
                case PortraitStatus.Idle: portrait.SetTrigger("InIdle"); break;
                case PortraitStatus.CurrentTurn: portrait.SetTrigger("InTurn"); break;
                case PortraitStatus.LookAt: portrait.SetTrigger("InLookAt"); break;
                case PortraitStatus.Dead: portrait.SetTrigger("InDead"); break;
            }
        }
    }

    private void FreeLookModeText(bool enabled)
    {
        freeLookTextrObject.SetActive(enabled);
    }
}
