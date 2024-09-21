using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public enum AIStates
{
    Idle, MoveTowards, MoveAway, Aim, Attack, UpdateEnemy
}
public class BaseAIManager : MonoBehaviour
{
    [Header("AI Status")]
    public AIStates _state = AIStates.Idle;
    public IControlleable _currentInControllCharacter;
    public List<BaseCharacter> _aiCharacters = new List<BaseCharacter>();
    public BaseCharacter _selectedEnemy;

    [Header("Chances settings")]
    public float _swapEnemyChances = 15;
    public float _moveTowardsEnemyChanches = 10;
    public float _moveAwayFromEnemyChances = 18;
    public float _aimChances = 25;
    private float _totalChances;

    [Header("Time settings")]
    [Range(0.5f, 3)] public float _minWaitTime = 0.5f;
    [Range(0.5f, 3)] public float _maxWaitTime = 1f;

    // Values
    Coroutine _waitTimer;
    Coroutine _movingTimer;
    Coroutine _aimTimer;
    public float _currentSwapEnemyChances;
    public float _currentMoveTowardsEnemyChances;
    public float _currentMoveAwayFromEnemyChances;
    public float _currentAimChances;

    public bool IsIdle => _waitTimer == null && _movingTimer == null && _aimTimer == null;
    public bool IsAITurn => _currentInControllCharacter != null;
    public float PlayerDirection => (_currentInControllCharacter.Position - _selectedEnemy.transform.position).normalized.x;

    private void Awake()
    {
        AIManagerEvents.OnUpdateAICharacters += OnCharactersListUpdate;
        AIManagerEvents.OnCharacterControlUpdate += OnControlledCharacterUpdated;

        _totalChances = _swapEnemyChances + _moveTowardsEnemyChanches + 
            _moveAwayFromEnemyChances; 
    }

    private void Start()
    {

    }

    private void OnDestroy()
    {
        AIManagerEvents.OnUpdateAICharacters -= OnCharactersListUpdate;
    }

    private void Update()
    {
        if (!IsAITurn)
        {
            _state = AIStates.Idle;
            ResetChances();
            StopTimers();
            return;
        }

        AIExecution(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!IsAITurn) return;

        ExecAI();
        AILateExecution(Time.deltaTime);
    }

    private void OnCharactersListUpdate(List<BaseCharacter> characters)
    {
        _aiCharacters = characters;
    }

    private void OnControlledCharacterUpdated(IControlleable newControlleable, bool inControl)
    {
        if (newControlleable == null) return;
        Debug.Log($"AI Character update: {newControlleable} - {inControl}");

        if (inControl) _currentInControllCharacter = newControlleable;
        else _currentInControllCharacter = null;
    }

    public virtual void AIExecution(float delta)
    {
        if (_selectedEnemy == null || _currentInControllCharacter == null) return;

        Debug.DrawLine(_currentInControllCharacter.Position, _selectedEnemy.transform.position, Color.cyan);
        Debug.DrawLine(_currentInControllCharacter.ProjectileOutPosition, _currentInControllCharacter.AimDirection * 20, Color.red);
    }

    public virtual void AILateExecution(float delta)
    {
        if (IsIdle || !IsAITurn) return;

        switch (_state)
        {
            case AIStates.MoveTowards:
                _currentInControllCharacter.InputMove(new Vector3(-PlayerDirection, 0));
                break;

            case AIStates.MoveAway:
                _currentInControllCharacter.InputMove(new Vector3(PlayerDirection, 0));
                break;

            case AIStates.Aim:
                AimToTarget();
                break;
        }
    }

    public virtual void ExecAI()
    {
        if (!IsIdle || !IsAITurn) return;

        if (_selectedEnemy == null) _state = AIStates.UpdateEnemy;

        switch (_state)
        {
            case AIStates.Idle:
                _waitTimer = StartCoroutine(WaitForIdle());
                break;
            case AIStates.MoveTowards:
                _movingTimer = StartCoroutine(WaitForMovement());
                break;
            case AIStates.MoveAway:
                _movingTimer = StartCoroutine(WaitForMovement());
                break;
            case AIStates.Aim:
                _aimTimer = StartCoroutine(WaitForAim());
                break;
            case AIStates.UpdateEnemy:
                _selectedEnemy = GameManager.GetRandomPlayerCharacterAlive();
                SetNextAction(Random.Range(0, _totalChances));
                break;
        }
    }

    // lucho cucurucho pone akakakakakakaka
    private void AimToTarget()
    {
        Vector3 direction = _selectedEnemy.transform.position - _currentInControllCharacter.ProjectileOutPosition;
        float distance = direction.magnitude;
        float heightDifference = _selectedEnemy.transform.position.y - _currentInControllCharacter.ProjectileOutPosition.y;

        float power = 1 * TestInGameUI.CurrentChargeBarPower; // gotta change this later


        _currentInControllCharacter.InputAim(new Vector3(0, -0.25f));
    }

    private void SetNextAction(float value)
    {
        float chancesValue = _currentSwapEnemyChances;

        if (value <= chancesValue)
        {
            _state = AIStates.UpdateEnemy;
            return;
        }

        chancesValue += _currentMoveAwayFromEnemyChances;
        if (value <= chancesValue)
        {
            _state = AIStates.MoveTowards;
            return;
        }

        chancesValue += _currentMoveTowardsEnemyChances;
        if (value <= chancesValue)
        {
            _state = AIStates.MoveAway;
            return;
        }

        else
        {
            _state = AIStates.Aim;
            return;
        }
    }

    private void StopTimers()
    {
        if (_waitTimer != null) _waitTimer = null;
        if (_movingTimer != null) _movingTimer = null;
        if (_aimTimer != null) _aimTimer = null;
    }

    private void ResetChances()
    {
        _currentSwapEnemyChances = _swapEnemyChances;
        _currentMoveAwayFromEnemyChances = _moveAwayFromEnemyChances;
        _currentMoveTowardsEnemyChances = _moveTowardsEnemyChanches;
        _currentAimChances = _aimChances;
    }

    IEnumerator WaitForIdle()
    {
        SetNextAction(Random.Range(0, _totalChances));
        yield return new WaitForSeconds(Random.Range(_minWaitTime, _minWaitTime + _maxWaitTime));
        Debug.Log("idle ended");
        _waitTimer = null;
    }

    IEnumerator WaitForMovement()
    {
        yield return new WaitForSeconds(Random.Range(_minWaitTime, _minWaitTime + _maxWaitTime));
        _currentMoveAwayFromEnemyChances -= Random.Range(1, _currentMoveAwayFromEnemyChances);
        _currentMoveTowardsEnemyChances -= Random.Range(1, _currentMoveAwayFromEnemyChances);

        if (_currentMoveAwayFromEnemyChances < 1) _currentMoveAwayFromEnemyChances = 1;
        if (_currentMoveTowardsEnemyChances < 1) _currentMoveTowardsEnemyChances = 1;

        Debug.Log("Moving ended");
        _state = AIStates.Idle;
        _movingTimer = null;
    }

    IEnumerator WaitForAim()
    {
        yield return new WaitForSeconds(Random.Range(_minWaitTime, _minWaitTime + _maxWaitTime));
        Debug.Log("aim ended");
        _state = AIStates.Idle;
        _aimTimer = null;
    }
}
