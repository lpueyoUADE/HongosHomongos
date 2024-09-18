using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTurnManager : MonoBehaviour
{
    [Header("References")]
    public Camera _camera;

    [Header("Settings")]
    public float _maxTurnTime = 15;
    public float _maxDelayBetweenTurns = 3;
    public float _cameraTransitionTime = 3;
    public Vector3 _cameraOffset = Vector3.zero;

    [Header("Playable characters list")]
    public List<BaseCharacter> _characters = new List<BaseCharacter>();
    public BaseCharacter _currentCharacterTurn;

    // Variables
    public int _roundedMaxTurnTime;

    // Status
    public bool _start = false;
    public Coroutine _turnTimer;
    public Coroutine _turnTimerShow;
    public Coroutine _timerBeforeNextTurn;

    public Vector3 _cameraPositionResult;
    public bool _moveCamera = false;

    private void Awake()
    {
        _roundedMaxTurnTime = (int)_maxTurnTime;
    }

    private void Start()
    {
        if (_camera == null) _camera = FindAnyObjectByType<Camera>();
        GetNextTurn();
        _start = true;

    }

    private void GetNextTurn()
    {
        // Nullify any pending timer
        if (_turnTimer != null) StopCoroutine(_turnTimer);
        if (_turnTimerShow != null) StopCoroutine(_turnTimerShow);
        if (_timerBeforeNextTurn != null) StopCoroutine(_timerBeforeNextTurn);
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
            _timerBeforeNextTurn = StartCoroutine(NextTurnTimer());
        }
    }

    private void EndTurn()
    {
        if (_currentCharacterTurn == null) return;

        // Disable control auth
        _currentCharacterTurn.InControl();
    }

    private void Update()
    {
        if (!_start) return;
    }

    private void FixedUpdate()
    {
        if (!_start) return;

        if (_moveCamera && _camera.transform.position != _cameraPositionResult)
            _camera.transform.position = Vector3.LerpUnclamped(_camera.transform.position, _cameraPositionResult, _cameraTransitionTime * 0.05f);

        if (_moveCamera && Vector3.Distance(_camera.transform.position, _cameraPositionResult) <= 0.25f) 
            _moveCamera = false;
    }

    IEnumerator TurnTime()
    {
        InGameUIEvents.OnUpdateTurnTime?.Invoke(_maxTurnTime.ToString());
        yield return new WaitForSeconds(_maxTurnTime);

        EndTurn();
        _turnTimer = null;
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
        yield return new WaitForSeconds(_maxTurnTime + _maxDelayBetweenTurns);
        _timerBeforeNextTurn = null;
        GetNextTurn();
    }
}
