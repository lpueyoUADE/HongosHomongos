using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTurnManager : MonoBehaviour
{
    [Header("Settings")]
    public float _maxTurnTime = 15;
    public float _maxDelayBetweenTurns = 3;
    public float _cameraTransitionTime = 3;
    public float _maxDelayAfterProjectileDeath = 4;
    public Vector3 _cameraOffset = Vector3.zero;

    [Header("Playable characters list")]
    public List<BaseCharacter> _characters = new List<BaseCharacter>();
    public BaseCharacter _currentCharacterTurn;

    [Header("Other values")]
    public int _roundedMaxTurnTime;

    [Header("Status")]
    public bool _start = false;
    public Vector3 _cameraPositionResult;
    public bool _moveCamera = false;

    // Timers
    public Coroutine _turnTimer;
    public Coroutine _turnTimerShow;
    public Coroutine _timerBeforeNextTurn;
    public Coroutine _timerBeforeExitingProjectileDamage;

    // Default events

    private void Awake()
    {
        GameTurnEvents.OnTurnEnd += EndTurn;
        GameTurnEvents.OnProjectileDeath += OnProjectileDeath;

        _roundedMaxTurnTime = (int)_maxTurnTime;
    }

    private void Start()
    {
        GetNextTurn();
        _start = true;
    }

    private void OnDestroy()
    {
        GameTurnEvents.OnTurnEnd -= EndTurn;
        GameTurnEvents.OnProjectileDeath -= OnProjectileDeath;
    }

    // Events
    private void OnProjectileDeath()
    {
        CameraEvents.OnCameraUpdateObjectToFollow(null);
        _timerBeforeExitingProjectileDamage = StartCoroutine(ProjectileDamageTimer());
    }

    // Turns

    private void GetNextTurn()
    {
        // Nullify any pending timer
        StopTimers();
        Debug.Log("Getting next turn...");

        // Add some checks for win/lose conditions..

        // First turn
        if (_currentCharacterTurn == null) _currentCharacterTurn = _characters[0];

        else
        {
            int index = _characters.IndexOf(_currentCharacterTurn);

            if (index + 1 >= _characters.Count) _currentCharacterTurn = _characters[0];
            else _currentCharacterTurn = _characters[index + 1];
        }

        // Check if the character is dead
        if (_currentCharacterTurn.IsDead)
        {
            GetNextTurn();
            return;
        }

        // Do turn stuff if not
        else
        {
            // Give control auth to character
            _currentCharacterTurn.InControl(true);

            // Set camera offsets
            _cameraPositionResult = _cameraOffset + _currentCharacterTurn.transform.position;
            _moveCamera = true;

            // Start timers
            _roundedMaxTurnTime = (int)_maxTurnTime;
            _turnTimer = StartCoroutine(TurnTime());
            _turnTimerShow = StartCoroutine(ShowTimer());
            CameraEvents.OnCameraUpdateObjectToFollow(_currentCharacterTurn.gameObject);
        }
    }

    private void EndTurn(IProjectile spawnedProjectile)
    {
        CameraEvents.OnCameraUpdateObjectToFollow(spawnedProjectile.Projectile);
        InGameUIEvents.OnTimerWait?.Invoke();
        StopTimers();
        EndTurnTimeOut();
    }

    private void EndTurnTimeOut()
    {
        if (_currentCharacterTurn == null) return;

        // Disable control auth
        _currentCharacterTurn.InControl();
    }

    // Timers
    private void StopTimers() 
    {
        if (_turnTimer != null) StopCoroutine(_turnTimer);
        if (_turnTimerShow != null) StopCoroutine(_turnTimerShow);
        if (_timerBeforeNextTurn != null) StopCoroutine(_timerBeforeNextTurn);
        if (_timerBeforeExitingProjectileDamage != null) StopCoroutine(_timerBeforeExitingProjectileDamage);
    }

    IEnumerator TurnTime()
    {
        InGameUIEvents.OnUpdateTurnTime?.Invoke(_maxTurnTime.ToString());
        yield return new WaitForSeconds(_maxTurnTime);

        EndTurnTimeOut();
        _turnTimer = null;
        _timerBeforeNextTurn = StartCoroutine(NextTurnTimer()); // After a turn ends, start a timer to get next turn
    }

    IEnumerator ShowTimer()
    {
        yield return new WaitForSeconds(1);

        if (_roundedMaxTurnTime > 0)
        {
            _roundedMaxTurnTime--;
            _turnTimerShow = StartCoroutine(ShowTimer());
            InGameUIEvents.OnUpdateTurnTime?.Invoke(_roundedMaxTurnTime.ToString());
        }

        else yield return null;
    }

    IEnumerator NextTurnTimer()
    {
        yield return new WaitForSeconds(_maxDelayBetweenTurns);
        _timerBeforeNextTurn = null;
        GetNextTurn();
    }

    IEnumerator ProjectileDamageTimer()
    {
        yield return new WaitForSeconds(_maxDelayAfterProjectileDeath);
        _timerBeforeExitingProjectileDamage = null;
        GetNextTurn();
    }
}
