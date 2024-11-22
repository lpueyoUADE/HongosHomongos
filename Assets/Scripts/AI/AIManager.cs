using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [SerializeField] private AIManagerTimersAndChances _managerData; 

    [Header("Status")]
    public List<BaseCharacter> _aiCharacters = new();
    public BaseCharacter _selectedEnemy;
    public IControlleable _currentInControllCharacter;
    public bool _isAiTurn = false;

    // FSM
    private AIManagerFSM<StatesEnum> _fsm;
    private ITreeNode _root;
    private RandomNode _randNextAction;
    private Dictionary<StatesEnum, ITreeNode> _randListedNodes = new();

    // Values
    Coroutine _waitTimer;
    Coroutine _moveTimer;
    Coroutine _aimTimer;
    Coroutine _chargeTimer;

    public bool IsAITurn => _isAiTurn;
    public IControlleable CurrentIControlleable => _currentInControllCharacter;
    public BaseCharacter CurrentControlledCharacter => _currentInControllCharacter.Character;
    public AICharacterConfig CurrentControlledCharacterData => (AICharacterConfig)_currentInControllCharacter.Character.CharacterData;
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
        AIManagerEvents.OnActionFinished += OnActionFinished;
    }

    private void UnsubFromEvents()
    {
        AIManagerEvents.OnUpdateAICharacters -= OnCharactersListUpdate;
        AIManagerEvents.OnCharacterControlUpdate -= OnControlledCharacterUpdated;
        AIManagerEvents.OnActionFinished -= OnActionFinished;
    }

    private void InitializeMachine()
    {
        _fsm = new AIManagerFSM<StatesEnum>();

        var idle = new AIStateIdle<StatesEnum>(this);
        var intentionalidle = new AIStateIntentionalIdle<StatesEnum>(this);
        var swaptarget = new AIStateSwapTarget<StatesEnum>(this);
        var move = new AIStateMove<StatesEnum>(this);
        var aim = new AIStateAim<StatesEnum>(this);
        var shoot = new AIStateFire<StatesEnum>(this);

        // Create list with all states
        Dictionary<StatesEnum, State<StatesEnum>> states = new Dictionary<StatesEnum, State<StatesEnum>>
        {
            { StatesEnum.Idle,                  idle                },
            { StatesEnum.IntentionalIdle,       intentionalidle     },
            { StatesEnum.SwapTarget,            swaptarget          },
            { StatesEnum.Move,                  move                },
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
        TestDebugBox.OnUpdateDebugBoxText?.Invoke($"AI FSMachine started.");
# endif
    }

    private void InitializeTree()
    {
        // Actions
        var idle = new ActionNode(() => _fsm.Transition(StatesEnum.Idle));
        var intentionalidle = new ActionNode(() => _fsm.Transition(StatesEnum.IntentionalIdle));
        var swaptarget = new ActionNode(() => _fsm.Transition(StatesEnum.SwapTarget));
        var move = new ActionNode(() => _fsm.Transition(StatesEnum.Move));
        var aim = new ActionNode(() => _fsm.Transition(StatesEnum.Aim));
        var shoot = new ActionNode(() => _fsm.Transition(StatesEnum.Fire));

        // Randomizable actions - characters may have their own modifier values
        var randNextAction = new Dictionary<ITreeNode, float>
        {
            [intentionalidle] = _managerData.ChancesIdle,
            [swaptarget] =      _managerData.ChancesSwapTarget,
            [move] =            _managerData.ChancesMove,
            [aim] =             _managerData.ChancesAim,
        };
        _randNextAction = new RandomNode(randNextAction);

        var randListedNodes = new Dictionary<StatesEnum, ITreeNode>
        {
            [StatesEnum.IntentionalIdle] =  intentionalidle,
            [StatesEnum.SwapTarget] =       swaptarget,
            [StatesEnum.Move] =             move,
            [StatesEnum.Aim] =              aim,
        };
        _randListedNodes = randListedNodes;

        // Tree of decisions
        var qCanDoAction = new QuestionNode(QuestionCanDoAction, _randNextAction, idle);
        var qIsIdling = new QuestionNode(QuestionIsIntentionallyIdling, intentionalidle, qCanDoAction);
        var qIsChargingShot = new QuestionNode(QuestionIsChargingShot, shoot, qIsIdling);
        var qIsAiming = new QuestionNode(QuestionIsAiming, aim, qIsChargingShot);
        var qIsMoving = new QuestionNode(QuestionMoving, move, qIsAiming);
        var qIsTargetValid = new QuestionNode(QuestionValidTarget, qIsMoving, swaptarget);
        var qIsAITurn = new QuestionNode(() => IsAITurn, qIsTargetValid, idle);

        _root = qIsAITurn;

# if UNITY_EDITOR
        TestDebugBox.OnUpdateDebugBoxText?.Invoke($"AI FSMachine Tree started.");
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

        // Get controlled character actions weight
        if (inControl)
        {
            _randNextAction.UpdateWeight(_randListedNodes[StatesEnum.IntentionalIdle], _randNextAction.GetWeight(_randListedNodes[StatesEnum.IntentionalIdle]) + CurrentControlledCharacterData.ChancesIdle);
            _randNextAction.UpdateWeight(_randListedNodes[StatesEnum.SwapTarget], _randNextAction.GetWeight(_randListedNodes[StatesEnum.SwapTarget]) + CurrentControlledCharacterData.ChancesSwapTarget);
            _randNextAction.UpdateWeight(_randListedNodes[StatesEnum.Move], _randNextAction.GetWeight(_randListedNodes[StatesEnum.Move]) + CurrentControlledCharacterData.ChancesMove);
            _randNextAction.UpdateWeight(_randListedNodes[StatesEnum.Aim], _randNextAction.GetWeight(_randListedNodes[StatesEnum.Aim]) + CurrentControlledCharacterData.ChancesAim);

# if UNITY_EDITOR
            TestDebugBox.OnUpdateDebugBoxText?.Invoke($"AI Updating new actions weight:");
            TestDebugBox.OnUpdateDebugBoxText?.Invoke($"{StatesEnum.IntentionalIdle} = {_randNextAction.GetWeight(_randListedNodes[StatesEnum.IntentionalIdle])}");
            TestDebugBox.OnUpdateDebugBoxText?.Invoke($"{StatesEnum.SwapTarget} = {_randNextAction.GetWeight(_randListedNodes[StatesEnum.SwapTarget])}");
            TestDebugBox.OnUpdateDebugBoxText?.Invoke($"{StatesEnum.Move} = {_randNextAction.GetWeight(_randListedNodes[StatesEnum.Move])}");
            TestDebugBox.OnUpdateDebugBoxText?.Invoke($"{StatesEnum.Aim} = {_randNextAction.GetWeight(_randListedNodes[StatesEnum.Aim])}");
# endif
        }

        _isAiTurn = inControl;
    }

    private void OnActionFinished(StatesEnum type)
    {
        if (!IsAITurn || !AnyTimerRunning()) return;

        switch (type)
        {
            case StatesEnum.Move:
                if (_moveTimer != null) _moveTimer = null;
                ActionWeightDivide(StatesEnum.Move, 3); // Lower even more the chances of moving if we already moved.
                break;
        }

# if UNITY_EDITOR
        TestDebugBox.OnUpdateDebugBoxText?.Invoke($"AI stopping {type} timer.");
# endif
    }

    // FSM Events
    public void IdleReset()
    {
        if (_waitTimer != null) _waitTimer = null;


        if (_aimTimer != null) _aimTimer = null;
        if (_chargeTimer != null) _chargeTimer = null;

        _randNextAction.ResetWeights();

# if UNITY_EDITOR
        TestDebugBox.OnUpdateDebugBoxText?.Invoke($"AI Reseting timers & values...");
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
        TestDebugBox.OnUpdateDebugBoxText?.Invoke($"AI Swapping enemy.");
# endif
    }

    public void StartTimer(StatesEnum type)
    {
        if (!IsAITurn || AnyTimerRunning()) return;
        float newTime = Random.Range(_managerData.TimeMinAction, _managerData.TimeMinAction + _managerData.TimeMaxAction);

        switch (type)
        {
            case StatesEnum.IntentionalIdle:
                newTime += Random.Range(CurrentControlledCharacterData.TimeMinAction, CurrentControlledCharacterData.TimeMaxAction);
                _waitTimer = StartCoroutine(TimerIdle(newTime));
                break;

            case StatesEnum.Move:
                newTime += Random.Range(CurrentControlledCharacterData.TimeMinMove, CurrentControlledCharacterData.TimeMinMove + CurrentControlledCharacterData.TimeMaxMove);
                _moveTimer = StartCoroutine(TimerMove(newTime));
                break;

            case StatesEnum.Aim:
                newTime = Random.Range(_managerData.TimeMinAim, _managerData.TimeMinAim + _managerData.TimeMaxAim);
                newTime += Random.Range(CurrentControlledCharacterData.TimeMinAim, CurrentControlledCharacterData.TimeMaxAim);
                _aimTimer = StartCoroutine(TimerAim(newTime));
                break;

            case StatesEnum.Fire:
                newTime = Random.Range(_managerData.TimeMinAim, _managerData.TimeMinAim + _managerData.TimeMaxAim);
                newTime += Random.Range(CurrentControlledCharacterData.TimeMinAim, CurrentControlledCharacterData.TimeMaxAim);
                newTime /= 2;
                _chargeTimer = StartCoroutine(TimerChargedShot(newTime));
                break;
        }

#if UNITY_EDITOR
        TestDebugBox.OnUpdateDebugBoxText?.Invoke($"AI {type} for {newTime}s...");
# endif
    }

    public bool AnyTimerRunning()
    {
        bool result = false;

        if (_waitTimer != null) result = true;
        if (_moveTimer != null) result = true;
        if (_aimTimer != null) result = true;
        if (_chargeTimer != null) result = true;

        return result;
    }

    // FSM Conditions
    private bool QuestionValidTarget()
    {
        return _selectedEnemy != null && IsAITurn;
    }

    private bool QuestionIsIntentionallyIdling()
    {
        return _waitTimer != null && IsAITurn;
    }

    private bool QuestionMoving()
    {
        return _moveTimer != null && IsAITurn;
    }

    private bool QuestionIsAiming()
    {
        return _aimTimer != null && IsAITurn;
    }

    private bool QuestionIsChargingShot()
    {
        return _chargeTimer != null && IsAITurn;
    }

    private bool QuestionCanDoAction()
    {
        return IsAITurn;
    }

    // Timers
    IEnumerator TimerIdle(float time)
    {
        yield return new WaitForSeconds(time);
        ActionWeightDivide(StatesEnum.IntentionalIdle, 2);
        _waitTimer = null;
    }

    IEnumerator TimerMove(float time)
    {
        yield return new WaitForSeconds(time);
        ActionWeightDivide(StatesEnum.Move, 2);
        _moveTimer = null;
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
        TestDebugBox.OnUpdateDebugBoxText?.Invoke($"AI Action new weight: {state} = {currentWeight / newWeightDivisor}.");
# endif
    }
}
