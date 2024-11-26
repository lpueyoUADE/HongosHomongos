using UnityEngine;

[CreateAssetMenu(fileName = "NewAreaAbilityData", menuName = "Databases/Character/Ability/AreaAbility")]
public class AreaAbilityData : CharacterAbilityData
{
    [Header("Area settings")]
    [SerializeField] private float _maxRadius = 10;
    [SerializeField] private bool _damageDecaysOnDistance = true;

    public float AbilityAreaRadius { get { return _maxRadius; } }
    public bool AbilityAreaDecayDamageByDistance { get { return _damageDecaysOnDistance; } }
}
