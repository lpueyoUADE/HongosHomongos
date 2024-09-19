using System;

public class InGameUIEvents
{
    private static float _chargeBarIntensity;

    public static Action<string> OnUpdateTurnTime;
    public static Action<bool> OnChargingWeaponBar;

    public static void UpdateChargeBarIntensity(float newAmount = 0)
    {
        _chargeBarIntensity = newAmount;
    }

    public static float GetChargerBarIntensity()
    {
        return _chargeBarIntensity;
    }
}
