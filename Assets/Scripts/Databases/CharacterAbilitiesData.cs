using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterAbilityData", menuName = "Databases/Character/Ability/SimpleAbility")]
public class CharacterAbilityData : ScriptableObject 
{
    [Header("Ability Settings")]
    [SerializeField] private string _abilityName = "AbilityName";
    [SerializeField] private BaseProjectile _prefab;
    [SerializeField] private Sprite _portraitSprite;
    [Tooltip("Charge bar speed.")] [SerializeField, Range(1, 5)] private float _chargeSpeed = 1.5f;
    [SerializeField] private int _levelRequiredForUnlock = 0;

    [Header("Projectile Settings")]
    [Tooltip("How much damage at impact.")] [SerializeField] private float _baseDamage = 2.2f;
    [Tooltip("How fast is at spawn.")][SerializeField] private float _baseSpeed = 20;
    [Tooltip("Time before self-destruct.")][SerializeField] private float _baseLife = 10;

    [Header("Other")]
    [Tooltip("When hitting something, this will spawn.")][SerializeField] private GameObject _residualObjectAtDeath;

    [Header("Audio Settings")]
    [SerializeField] private List<AudioClip> _shootAudioClips = new();
    [SerializeField] private List<AudioClip> _destroyedSounds = new();

    public string AbilityName { get { return _abilityName; } }
    public BaseProjectile AbilitProjectilePrefab { get { return _prefab; } }
    public Sprite AbilityPortraitSprite { get { return _portraitSprite; } }
    public float AbilitProjectileChargingSpeed { get { return _chargeSpeed; } }
    public int AbilitLevelRequired { get { return _levelRequiredForUnlock; } }

    public float AbilityProjectileBaseDamage { get { return _baseDamage; } }
    public float AbilityProjectileBaseSpeed { get { return _baseSpeed; } }
    public float AbilityProjectileBaseLife { get { return _baseLife; } }

    public GameObject AbilityResidualPrefab { get { return _residualObjectAtDeath; } }

    public AudioClip AbilitFireSound { get { return _shootAudioClips[Random.Range(0, _shootAudioClips.Count)]; } }
    public AudioClip AbilitDestroyedSound { get { return _destroyedSounds[Random.Range(0, _destroyedSounds.Count)]; } }
}
