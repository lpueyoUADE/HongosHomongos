using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShieldAbilityData", menuName = "Databases/Character/Ability/ShieldAbility", order = 0)]
public class ShieldAbilityData : CharacterAbilityData
{
    [Header("Shield Settings")]
    [SerializeField, Range(1, 100)] private int _turnsDuration = 3;
    [SerializeField, Range(1, 100)] private int _damageReductionPercent = 40;

    public int ShieldTurnsDuration { get { return _turnsDuration; } }
    public int ShieldDamageReductionPercent { get { return _damageReductionPercent; } }
}