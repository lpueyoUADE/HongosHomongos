using System;
using UnityEngine;


public enum PortraitStatus {
    Idle, CurrentTurn, Dead
}
public class InGameUIEvents
{
    private static float _chargeBarIntensity;

    public static Action OnTimerWait;
    public static Action<string, bool> OnUpdateTurnTime;
    public static Action<bool> OnChargingWeaponBar;
    public static Action<AudioClip> OnPlayUISound;
    public static Action<GameObject> OnAddCharacterPortrait;
    public static Action<int, PortraitStatus> OnPortraitUpdate;

    public static void UpdateChargeBarIntensity(float newAmount = 0)
    {
        _chargeBarIntensity = newAmount;
    }

    public static float GetChargerBarIntensity()
    {
        return _chargeBarIntensity;
    }
}
