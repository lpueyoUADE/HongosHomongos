using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbilities
{
    void OnAbilityHeal(float amount);
    void OnAbilityTimedDamage(float amount, int duration);
    void OnAbilityShield(int turnDurations, GameObject shieldPrefab, int damageReductionPercet);
    void OnAbilityShieldCheck();
    void OnWeakened(int turnDurations, int extraDamageReceived);
    void OnStrengthened(int turnDurations, int extraDamageResistance);
}
