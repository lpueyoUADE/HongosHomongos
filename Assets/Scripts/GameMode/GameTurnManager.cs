using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTurnManager : MonoBehaviour
{
    [Header("Status")]
    private static List<BaseCharacter> _characters = new ();
    public int _roundedMaxTurnTime;
    public static BaseCharacter _currentCharacterTurn;
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
        GameTurnEvents.OnCharactersListUpdate += InitializeTurns;
        GameTurnEvents.OnGameEnded += GameEnded;
    }

    private void Start() 
    {
        _roundedMaxTurnTime = (int)GameManagerEvents.ModeSettings.MaxDelayBetweenTurns;
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void UnsubscribeFromEvents()
    {
        GameTurnEvents.OnTurnEnd -= EndTurn;
        GameTurnEvents.OnProjectileDeath -= OnProjectileDeath;
        GameTurnEvents.OnCharactersListUpdate -= InitializeTurns;
        GameTurnEvents.OnGameEnded -= GameEnded;
    }

    // Events
    private void OnProjectileDeath()
    {
        CameraEvents.OnCameraUpdateObjectToFollow(null, false);
        _timerBeforeExitingProjectileDamage = StartCoroutine(ProjectileDamageTimer());
    }

    private void InitializeTurns(List<BaseCharacter> newCharactersList)
    {
        _characters = newCharactersList;
        GetNextTurn();
    }

    private void GameEnded()
    {
# if UNITY_EDITOR
        string msg = "GAME ENDED - TURN MANAGER STOPPING...";
        TestDebugBox.OnUpdateDebugBoxText?.Invoke(msg);
        Debug.Log(msg);
# endif

        StopTimers();
        UnsubscribeFromEvents();
    }

    // Turns
    private void GetNextTurn()
    {
        // Nullify any pending timer
        StopTimers();

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
            InGameUIEvents.OnPortraitUpdate(_characters.IndexOf(_currentCharacterTurn), PortraitStatus.Dead);
            GetNextTurn();
            return;
        }

        // Do turn stuff if not
        else
        {
            // Give control auth to character
            _currentCharacterTurn.InControl(true);
            InGameUIEvents.OnPortraitUpdate(_characters.IndexOf(_currentCharacterTurn), PortraitStatus.CurrentTurn);

            // Set camera offsets
            _cameraPositionResult = GameManagerEvents.ModeSettings.InGameCameraOffset + _currentCharacterTurn.transform.position;
            _moveCamera = true;

            // Start timers
            _roundedMaxTurnTime = (int)GameManagerEvents.ModeSettings.MaxTurnTime;
            _turnTimer = StartCoroutine(TurnTime());
            _turnTimerShow = StartCoroutine(ShowTimer());
            CameraEvents.OnCameraUpdateObjectToFollow(_currentCharacterTurn.gameObject, true);

            GameTurnEvents.OnTurnStart?.Invoke();
        }
    }

    private void EndTurn(IProjectile spawnedProjectile)
    {
        InGameUIEvents.OnTimerWait?.Invoke();
        StopTimers();
        EndTurnTimeOut();

        if (spawnedProjectile != null) CameraEvents.OnCameraUpdateObjectToFollow(spawnedProjectile.Projectile, false);
        else _timerBeforeNextTurn = StartCoroutine(NextTurnTimer());
    }

    private void EndTurnTimeOut()
    {
        if (_currentCharacterTurn == null) return;

        GameTurnEvents.OnTurnEndManager?.Invoke();
        // Disable control auth
        _currentCharacterTurn.InControl();
        InGameUIEvents.OnPortraitUpdate(_characters.IndexOf(_currentCharacterTurn), PortraitStatus.Idle);
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
        InGameUIEvents.OnUpdateTurnTime?.Invoke(GameManagerEvents.ModeSettings.MaxTurnTime.ToString(), true);
        yield return new WaitForSeconds(GameManagerEvents.ModeSettings.MaxTurnTime);

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
            InGameUIEvents.OnUpdateTurnTime?.Invoke(_roundedMaxTurnTime.ToString(), true);
        }

        else yield return null;
    }

    IEnumerator NextTurnTimer()
    {
        yield return new WaitForSeconds(GameManagerEvents.ModeSettings.MaxDelayBetweenTurns);
        _timerBeforeNextTurn = null;
        GetNextTurn();
    }

    IEnumerator ProjectileDamageTimer()
    {
        yield return new WaitForSeconds(GameManagerEvents.ModeSettings.MaxDelayAfterProjectileDeath);
        _timerBeforeExitingProjectileDamage = null;
        GetNextTurn();
    }

    public static GameObject GetCurrentCharacterTurn()
    {
        return _currentCharacterTurn.gameObject;
    }

    public static GameObject GetCharacter(int index)
    {
        if (_characters.Count <= index) return null;

        GameObject character = _characters[index].gameObject;
        return character;
    }
}
