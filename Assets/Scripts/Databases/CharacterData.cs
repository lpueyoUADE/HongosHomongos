using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Databases/Character/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Character settings")]
    [SerializeField] private float _life = 10;
    [SerializeField, Range(0.1f, 2)] private float _speed = 0.45f;
    [SerializeField, Range(0, 20)] private float _jumpForce = 12;
    [SerializeField] private LayerMask _floorMask;
    [SerializeField] private bool _isAIControlled = false;
    [SerializeField] private Sprite _characterPortrait;

    [Header("Falling settings")]
    [SerializeField, Range(0, 1)] private float _fallSpeedModifier = 0.45f;
    [SerializeField, Range(25, 50)] private float _fallMaxSpeed = 35;
    [SerializeField, Range(0.5f, 10)] private float _fallDamageMinDistance = 6;
    [SerializeField, Range(0.1f, 10)] private float _fallDamageMultiplier = 4;
    [SerializeField, Range(0.1f, 0.5f)] private float _fallMinVelocityTolerance = 0.25f;

    [Header("Abilities")]
    [SerializeField] private List<CharacterAbilityData> _abilitiesList = new();

    [Header("Sounds")]
    [SerializeField] private AudioClip _deathSound;
    [SerializeField] private AudioClip _fallDamageSound;
    [SerializeField] private AudioClip _jumpSound;
    [SerializeField] private AudioClip _groundSound;
    [SerializeField] private List<AudioClip> _anyDamageSound;

    [Header("Other")]
    [SerializeField] private GameObject _onHitParticles;

    public float Life { get { return _life;} }
    public float Speed { get { return _speed;} }
    public float JumpForce { get { return _jumpForce;} }
    public LayerMask FloorMask { get {return _floorMask;} }
    public bool IsAIControlled { get { return _isAIControlled;} }
    public Sprite Portrait { get { return _characterPortrait; } }


    public float FallSpeedModifier { get { return _fallSpeedModifier; } }
    public float FallMaxSpeed { get { return _fallMaxSpeed; } }
    public float FallDamageMinDistance { get { return _fallDamageMinDistance; } }
    public float FallDamageMultiplier { get { return _fallDamageMultiplier; } }
    public float FallMinVelocityTolerance { get { return _fallMinVelocityTolerance; } }

    public List<CharacterAbilityData> AbilitiesList { get { return _abilitiesList; } }

    public AudioClip DeathSound { get { return _deathSound; } }
    public AudioClip FallDamageSound { get { return _fallDamageSound; } }
    public AudioClip JumpSound { get { return _jumpSound; } }
    public AudioClip GroundSound { get { return _groundSound; } }
    public AudioClip AnyDamageSound { get { return _anyDamageSound[Random.Range(0, _anyDamageSound.Count)]; } }
    public GameObject OnDamageBloodParticles { get { return _onHitParticles; } }

}
