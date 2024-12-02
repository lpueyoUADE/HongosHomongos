
using UnityEngine;

[CreateAssetMenu(fileName = "NewAIManagerTimersAndChances", menuName = "Databases/AIManager")]
public class AIManagerTimersAndChances : ScriptableObject 
{
    [Header("Behavior values")]
    [SerializeField, Range(1, 3)] private float _intentionalIdleChances = 1;
    [SerializeField, Range(1, 3)] private float _swapTargetChances = 2;
    [SerializeField, Range(0, 3)] private float _moveChances = 1;
    [SerializeField, Range(1, 3)] private float _aimChances = 3;

    [Header("Time settings")]
    [SerializeField, Range(.1f, 1)] private float _minActionTime = .25f;
    [SerializeField, Range(.1f, 1)] private float _maxActionTime = .25f;
    [SerializeField, Range(.1f, 1)] private float _minShootTimeWait = .25f;
    [SerializeField, Range(.1f, 1)] private float _maxShootTimeWait = .25f;

    public float ChancesIdle { get { return _intentionalIdleChances; } }
    public float ChancesSwapTarget { get { return _swapTargetChances; } }
    public float ChancesMove { get { return _moveChances; } }
    public float ChancesAim { get { return _aimChances; } }

    public float TimeMinAction { get { return _minActionTime; } }
    public float TimeMaxAction { get { return _maxActionTime; } }
    public float TimeMinAim { get { return _minShootTimeWait; } }
    public float TimeMaxAim { get { return _maxShootTimeWait; } }
}
