using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAICharacterData", menuName = "Databases/Character/AICharacterData", order = 0)]
public class AICharacterConfig : CharacterData 
{
    [Header("Behavior values")]
    [SerializeField, Range(1, 20)] private float _intentionalIdleChances = 2;
    [SerializeField, Range(1, 20)] private float _swapTargetChances = 5;
    [SerializeField, Range(1, 20)] private float _moveChances = 3;
    [SerializeField, Range(1, 20)] private float _aimChances = 8;

    [Header("Time settings")]
    [SerializeField, Range(.5f, 3)] private float _minActionTime = .75f;
    [SerializeField, Range(.5f, 3)] private float _maxActionTime = 1;
    [SerializeField, Range(2, 5)] private float _minMoveTime = 3;
    [SerializeField, Range(2, 5)] private float _maxMoveTime = 4;
    [SerializeField, Range(.1f, 1)] private float _minShootTimeWait = .75f;
    [SerializeField, Range(.9f, 2)] private float _maxShootTimeWait = 1.5f;


    public float ChancesIdle { get { return _intentionalIdleChances; } }
    public float ChancesSwapTarget { get { return _swapTargetChances; } }
    public float ChancesMove { get { return _moveChances; } }
    public float ChancesAim { get { return _aimChances; } }

    public float TimeMinAction { get { return _minActionTime; } }
    public float TimeMaxAction { get { return _maxActionTime; } }
    public float TimeMinMove { get { return _minMoveTime; } }
    public float TimeMaxMove { get { return _maxMoveTime; } }
    public float TimeMinAim { get { return _minShootTimeWait; } }
    public float TimeMaxAim { get { return _maxShootTimeWait; } }

    public Dictionary<StatesEnum, float> GetBehaviorValues()
    {
        return new Dictionary<StatesEnum, float>
        {
            [StatesEnum.IntentionalIdle]    = _intentionalIdleChances,
            [StatesEnum.SwapTarget]         = _swapTargetChances,
            [StatesEnum.Move]               = _moveChances,
            [StatesEnum.Aim]                = _aimChances,
        };
    }
}