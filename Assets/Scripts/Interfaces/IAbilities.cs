using UnityEngine;

public interface IAbilities
{
    void OnAbilityHeal(float amount);
    void OnAbilityTimedDamage(float amount, int duration);
    void OnAbilityShield(int turnDurations, GameObject shieldPrefab, int damageReductionPercet);
    void OnAbilityTurnCheck();
    void OnWeakened(int turnDurations, float extraDamageReceived, GameObject particles = null);
    void OnStrengthened(int turnDurations, float extraDamageResistance);
}
