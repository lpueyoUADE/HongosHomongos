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
    public AudioClip _timeClickSound;
    public AudioClip _timeEndSound;

    [Header("Charging bar")]
    public Image _chargingWeaponBar;
    public float _chargeWeaponBarValue;     // Sets charging bar fill amount
    public bool _chargeWeaponBar;           // Show/Hide charging bar
    public float _chargeWeaponBarStartTime; // Makes charging bar reset when ping poing
    public bool _lockWeaponChargeBar;       // Makes ping pong work

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();

        InGameUIEvents.OnUpdateTurnTime += UpdateTurnTime;
        InGameUIEvents.OnChargingWeaponBar += ChargingBar;
    }

    private void Start()
    {
        _chargeWeaponBarStartTime = Time.time;
    }

    private void OnDestroy()
    {
        InGameUIEvents.OnUpdateTurnTime -= UpdateTurnTime;
        InGameUIEvents.OnChargingWeaponBar -= ChargingBar;
    }

    private void Update()
    {
        if (_chargeWeaponBar)
        {
            if (!_lockWeaponChargeBar) _chargeWeaponBarStartTime = Time.time;

            _lockWeaponChargeBar = true;
            _chargeWeaponBarValue = (Time.time - _chargeWeaponBarStartTime) * 2;
            _chargingWeaponBar.fillAmount = Mathf.PingPong(_chargeWeaponBarValue, 1);
            InGameUIEvents.UpdateChargeBarIntensity(_chargingWeaponBar.fillAmount); // Fill amount is used to get values from 0 to 1.
        }
        
        else
        {
            _chargeWeaponBarValue = 0;
            _chargingWeaponBar.fillAmount = 0;
            _lockWeaponChargeBar = false;
            InGameUIEvents.UpdateChargeBarIntensity();
        }
    }

    private void UpdateTurnTime(string newTime)
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

    private void ChargingBar(bool show)
    {
        _chargeWeaponBar = show;
        _chargingWeaponBar.gameObject.SetActive(show);
    }
}
