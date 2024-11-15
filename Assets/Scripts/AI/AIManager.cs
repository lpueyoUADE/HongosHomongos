using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [Header("Time settings")]
    [Range(0.5f, 3)] public float _minActionTime = 0.5f;
    [Range(0.5f, 3)] public float _maxActionTime = 1;
    [Range(0.1f, 3)] public float _minShootTimeWait = 1;
    [Range(0.9f, 3)] public float _maxShootTimeWait = 1;

    [Header("Decision making")]
    [Range(1, 20)] public float _intentionalIdleChances = 2;
    [Range(1, 20)] public float _swapTargetChances = 5;
    [Range(1, 20)] public float _moveTowardsChances = 10;
    [Range(1, 20)] public float _moveAwayChances = 10;
    [Range(1, 20)] public float _aimChances = 8;

    [Header("Status")]
    public List<BaseCharacter> _aiCharacters = new List<BaseCharacter>();
    public BaseCharacter _selectedEnemy;
    public IControlleable _currentInControllCharacter;
    public bool _isAiTurn = false;

    // FSM
    private AIManagerFSM<StatesEnum> _fsm;
    private ITreeNode _root;
    private RandomNode _randNextAction;
    private Dictionary<StatesEnum, ITreeNode> _randListedNodes = new Dictionary<StatesEnum, ITreeNode>();

    // Values
    Coroutine _waitTimer;
    Coroutine _movingForwardTimer; 
    Coroutine _movingAwayTimer;
    Coroutine _aimTimer;
    Coroutine _chargeTimer;

    public bool IsAITurn => _isAiTurn;
    public IControlleable CurrentIControlleable => _currentInControllCharacter;
    public BaseCharacter CurrentControlledCharacter => _currentInControllCharacter.Character;
    public Transform TargetTransform => _selectedEnemy.transform;
    public Vector3 TargetPosition => _selectedEnemy.transform.position;
    public float TargetDirection => (CurrentControlledCharacter.CharacterPosition - TargetPosition).normalized.x;

    private void Awake()
    {
        SubToEvents();
    }

    private void OnDestroy()
    {
        UnsubFromEvents();
    }

    private void Start()
    {
        InitializeMachine();
        InitializeTree();
    }

    private void Update()
    {
        _fsm?.OnUpdate();
        _root?.Execute();
    }

    private void FixedUpdate()
    {
        _fsm?.OnLateUpdate();
    }

    private void SubToEvents()
    {
        AIManagerEvents.OnUpdateAICharacters += OnCharactersListUpdate;
        AIManagerEvents.OnCharacterControlUpdate += OnControlledCharacterUpdated;
    }

    private void UnsubFromEvents()
    {
        AIManagerEvents.OnUpdateAICharacters -= OnCharactersListUpdate;
        AIManagerEvents.OnCharacterControlUpdate -= OnControlledCharacterUpdated;
    }

    private void InitializeMachine()
    {
        _fsm = new AIManagerFSM<StatesEnum>();

        var idle = new AIStateIdle<StatesEnum>(this);
        var intentionalidle = new AIStateIntentionalIdle<StatesEnum>(this);
        var swaptarget = new AIStateSwapTarget<StatesEnum>(this);
        var movetowards = new AIStateMoveTowards<StatesEnum>(this);
        var moveaway = new AIStateMoveAway<StatesEnum>(this);
        var aim = new AIStateAim<StatesEnum>(this);
        var shoot = new AIStateFire<StatesEnum>(this);

        // Create list with all states
        Dictionary<StatesEnum, State<StatesEnum>> states = new Dictionary<StatesEnum, State<StatesEnum>>
        {
            { StatesEnum.Idle,                  idle                },
            { StatesEnum.IntentionalIdle,       intentionalidle     },
            { StatesEnum.SwapTarget,            swaptarget          },
            { StatesEnum.MoveTowards,           movetowards         },
            { StatesEnum.MoveAway,              moveaway            },
            { StatesEnum.Aim,                   aim                 },
            { StatesEnum.Fire,                  shoot               },
        };

        // Add states to each other
        foreach (var state in states) 
            foreach (var transitionState in states)
                state.Value.AddTransition(transitionState.Key, transitionState.Value);

        // Set initial state
        _fsm.SetInit(idle);

# if UNITY_EDITOR
        TestDebugBox.OnUpdateDebugBoxText?.Invoke($"Machine initialized.");
# endif
    }

    private void InitializeTree()
    {
        // Actions
        var idle = new ActionNode(() => _fsm.Transition(StatesEnum.Idle));
        var intentionalidle = new ActionNode(() => _fsm.Transition(StatesEnum.IntentionalIdle));
        var swaptarget = new ActionNode(() => _fsm.Transition(StatesEnum.SwapTarget));
        var movetowards = new ActionNode(() => _fsm.Transition(StatesEnum.MoveTowards));
        var moveaway = new ActionNode(() => _fsm.Transition(StatesEnum.MoveAway));
        var aim = new ActionNode(() => _fsm.Transition(StatesEnum.Aim));
        var shoot = new ActionNode(() => _fsm.Transition(StatesEnum.Fire));

        // Randomizable actions - characters may have their own modifier values
        var randNextAction = new Dictionary<ITreeNode, float>
        {
            [intentionalidle] = _intentionalIdleChances,
            [swaptarget] =      _swapTargetChances,
            [movetowards] =     _moveTowardsChances,
            [moveaway] =        _moveAwayChances,
            [aim] =             _aimChances,
        };
        _randNextAction = new RandomNode(randNextAction);

        var randListedNodes = new Dictionary<StatesEnum, ITreeNode>
        {
            [StatesEnum.IntentionalIdle] =  intentionalidle,
            [StatesEnum.SwapTarget] =       swaptarget,
            [StatesEnum.MoveTowards] =      movetowards,
            [StatesEnum.MoveAway] =         moveaway,
            [StatesEnum.Aim] =              aim,
        };
        _randListedNodes = randListedNodes;

        // Tree of decisions
        var qIsIdling = new QuestionNode(QuestionIsIntentionallyIdling, intentionalidle, _randNextAction);
        var qIsChargingShot = new QuestionNode(QuestionIsChargingShot, shoot, qIsIdling);
        var qIsAiming = new QuestionNode(QuestionIsAiming, aim, qIsChargingShot);
        var qIsMovingAway = new QuestionNode(QuestionMovingAway, moveaway, qIsAiming);
        var qIsMovingTowards = new QuestionNode(QuestionMovingTowards, movetowards, qIsMovingAway);
        var qIsTargetValid = new QuestionNode(QuestionValidTarget, qIsMovingTowards, swaptarget);
        var qIsAITurn = new QuestionNode(() => IsAITurn, qIsTargetValid, idle);

        _root = qIsAITurn;

# if UNITY_EDITOR
        TestDebugBox.OnUpdateDebugBoxText?.Invoke($"Tree initialized.");
# endif
    }

    // Actions
    private void OnCharactersListUpdate(List<BaseCharacter> characters)
    {
        _aiCharacters = characters;
    }

    private void OnControlledCharacterUpdated(IControlleable newControlleable, bool inControl)
    {
        if (inControl) _currentInControllCharacter = newControlleable;
        IdleReset();
        _isAiTurn = inControl;
    }

    // FSM Events
    public void IdleReset()
    {
        if (_waitTimer != null) _waitTimer = null;
        if (_movingForwardTimer != null) _movingForwardTimer = null;
        if (_movingAwayTimer != null) _movingAwayTimer = null;
        if (_aimTimer != null) _aimTimer = null;
        if (_chargeTimer != null) _chargeTimer = null;

        _randNextAction.ResetWeights();

# if UNITY_EDITOR
        TestDebugBox.OnUpdateDebugBoxText?.Invoke($"Reseting timers & values...");
# endif
    }

    public void ForceStopTurn()
    {
        _isAiTurn = false;
    }

    public void SwapTarget()
    {
        if (!IsAITurn || QuestionIsChargingShot() || QuestionIsAiming()) return;

        _selectedEnemy = GameManager.GetRandomPlayerCharacterAlive();
#if UNITY_EDITOR
        TestDebugBox.OnUpdateDebugBoxText?.Invoke($"Swapping enemy.");
# endif
    }

    public void StartTimer(StatesEnum type)
    {
        if (!IsAITurn || AnyTimerRunning()) return;
        float newTime = Random.Range(_minActionTime, _minActionTime + _maxActionTime);

        switch (type)
        {
            case StatesEnum.IntentionalIdle:
                _waitTimer = StartCoroutine(TimerIdle(newTime));
                break;

            case StatesEnum.MoveTowards:
                _movingForwardTimer = StartCoroutine(TimerMoveTowards(newTime));
                break;

            case StatesEnum.MoveAway:
                _movingAwayTimer = StartCoroutine(TimerMoveAway(newTime));
                break;

            case StatesEnum.Jump:
                break;

            case StatesEnum.Drop:
                break;

            case StatesEnum.Aim:
                newTime = Random.Range(_minShootTimeWait, _minShootTimeWait + _maxShootTimeWait);
                _aimTimer = StartCoroutine(TimerAim(newTime));
                break;

            case StatesEnum.Fire:
                newTime = Random.Range(_minShootTimeWait, _minShootTimeWait + _maxShootTimeWait);
                _chargeTimer = StartCoroutine(TimerChargedShot(newTime));
                break;
        }

#if UNITY_EDITOR
        TestDebugBox.OnUpdateDebugBoxText?.Invoke($"{type} for {newTime}s...");
# endif
    }

    public bool AnyTimerRunning()
    {
        bool result = false;

        if (_waitTimer != null) result = true;
        if (_movingForwardTimer != null) result = true;
        if (_movingAwayTimer != null) result = true;
        if (_aimTimer != null) result = true;
        if (_chargeTimer != null) result = true;

        return result;
    }

    // FSM Conditions
    private bool QuestionValidTarget()
    {
        return _selectedEnemy != null;
    }

    private bool QuestionIsIntentionallyIdling()
    {
        return _waitTimer != null;
    }

    private bool QuestionMovingTowards()
    {
        return _movingForwardTimer != null;
    }

    private bool QuestionMovingAway()
    {
        return _movingAwayTimer != null;
    }

    private bool QuestionIsAiming()
    {
        return _aimTimer != null;
    }

    private bool QuestionIsChargingShot()
    {
        return _chargeTimer != null;
    }

    // Timers
    IEnumerator TimerIdle(float time)
    {
        yield return new WaitForSeconds(time);
        ActionWeightDivide(StatesEnum.IntentionalIdle, 2);
        _waitTimer = null;
    }

    IEnumerator TimerMoveTowards(float time)
    {
        yield return new WaitForSeconds(time);
        ActionWeightDivide(StatesEnum.MoveTowards, 2);
        _movingForwardTimer = null;
    }

    IEnumerator TimerMoveAway(float time)
    {
        yield return new WaitForSeconds(time);
        ActionWeightDivide(StatesEnum.MoveAway, 2);
        _movingAwayTimer = null;
    }

    IEnumerator TimerAim(float time)
    {
        yield return new WaitForSeconds(time);
        _aimTimer = null;
    }

    IEnumerator TimerChargedShot(float time)
    {
        yield return new WaitForSeconds(time);
        _chargeTimer = null;
    }
    
    // Modify node weights to prevent spamming some actions...
    private void ActionWeightDivide(StatesEnum state, float newWeightDivisor)
    {
        if (!_randListedNodes.ContainsKey(state) || !IsAITurn) return;

        float currentWeight = _randNextAction.GetWeight(_randListedNodes[state]);
        _randNextAction.UpdateWeight(_randListedNodes[state], currentWeight / newWeightDivisor);

#if UNITY_EDITOR
        TestDebugBox.OnUpdateDebugBoxText?.Invoke($"Action new weight: {state} = {currentWeight / newWeightDivisor}.");
# endif
    }
}
