using UnityEngine;


[CreateAssetMenu(fileName = "NewGameModeGeneralSettings", menuName = "Databases/GameModeGeneralSettings")]
public class GameModeGeneralSettings : ScriptableObject 
{
    [Header("Databases")]
    [SerializeField] private CharacterNames _characterNames;

    [Header("General sounds")]
    [SerializeField] private AudioClip _clockTickSound;
    [SerializeField] private AudioClip _clockEndTurnSound;
    [SerializeField] private AudioClip _portraitLookStartSound;
    [SerializeField] private AudioClip _portraitLookEndSound;
    [SerializeField] private AudioClip _abilityChangeSound;

    [Header("Before fight introduction times")]
    [SerializeField, Range(1, 5)] private float _delayBetweenCharacters = 2;
    [SerializeField, Range(1, 5)] private float _cameraTransitionTime = 2;
    [SerializeField, Range(2, 5)] private float _delayBeforeStartingGame = 3.5f;
    [SerializeField] private AudioClip _characterIntroductionClip;

    [Header("Turn System timers")]
    [SerializeField, Range(5, 25)] private float _maxTurnTime = 15;
    [SerializeField, Range(1, 5)] private float _maxDelayBetweenTurns = 3;
    [SerializeField, Range(1, 5)] private float _turnCameraTransitionTime = 3;
    [SerializeField, Range(1, 5)] private float _maxDelayAfterProjectileDeath = 4;

    [Header("AI General Chances")]

    [Header("InGame camera settings")]
    [SerializeField] private Vector3 _inGameCameraOffset = new(0, 0, -10);

    public CharacterNames CharacterNamesDatabase { get { return _characterNames;} }

    public AudioClip ClockTickSound { get { return _clockTickSound;} }
    public AudioClip ClockEndTurnSound { get { return _clockEndTurnSound;} }
    public AudioClip PortraitLookStartSound { get { return _portraitLookStartSound;} }
    public AudioClip PortraitLookEndSound { get { return _portraitLookEndSound;} }
    public AudioClip AbilityChangeSound { get { return _abilityChangeSound;} }

    public float DelayBetweenCharacters { get { return _delayBetweenCharacters;} }
    public float CameraTransitionTime { get { return _cameraTransitionTime;} }
    public float DelayBeforeStartingGame { get { return _delayBeforeStartingGame;} }
    public AudioClip CharacterIntroductionAudioClip { get { return _characterIntroductionClip;} }

    public float MaxTurnTime { get { return _maxTurnTime;} }
    public float MaxDelayBetweenTurns { get { return _maxDelayBetweenTurns;} }
    public float TurnCameraTransitionTime { get { return _turnCameraTransitionTime;} }
    public float MaxDelayAfterProjectileDeath { get { return _maxDelayAfterProjectileDeath;} }

    public Vector3 InGameCameraOffset { get { return _inGameCameraOffset;} }
}
