using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewGameModeGeneralSettings", menuName = "Databases/GameModeGeneralSettings")]
public class GameModeGeneralSettings : ScriptableObject 
{
    [Header("General sounds")]
    [SerializeField] private AudioClip _clockTickSound;
    [SerializeField] private AudioClip _clockEndTurnSound;


    [Header("Before fight introduction times")]
    [SerializeField, Range(1, 5)] private float _delayBetweenCharacters = 2;
    [SerializeField, Range(1, 5)] private float _cameraTransitionTime = 2;
    [SerializeField, Range(2, 5)] private float _delayBeforeStartingGame = 3.5f;
    [SerializeField] private AudioClip _characterIntroductionClip;

    [Header("Turn System timers")]
    [SerializeField] private float _maxTurnTime = 15;
    [SerializeField] private float _maxDelayBetweenTurns = 3;
    [SerializeField] private float _turnCameraTransitionTime = 3;
    [SerializeField] private float _maxDelayAfterProjectileDeath = 4;

    [Header("InGame camera settings")]
    [SerializeField] private Vector3 _inGameCameraOffset = new Vector3(0, 0, -10);

    public AudioClip ClockTickSound { get { return _clockTickSound;} }
    public AudioClip ClockEndTurnSound { get { return _clockEndTurnSound;} }

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
