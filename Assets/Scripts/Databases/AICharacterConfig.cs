using UnityEngine;

[CreateAssetMenu(fileName = "NewAICharacterData", menuName = "Databases/Character/AICharacterData", order = 0)]
public class AICharacterConfig : CharacterData 
{
    [Header("AI jump & fall settings")]
    [SerializeField, Range(.1f, 2)] private float _fallDetectionOriginDistance = 0.5f;
    [SerializeField, Range(.1f, 5)] private float _fallDetectionVerticalMinDistance = 3;
    [SerializeField, Range(.01f, .5f)] private float _maxJumpDistanceMultiplier = .2f;
    [SerializeField, Range(.1f, 4)] private float _closerFloorHeightMinDistance = 1.5f;

    [Header("Node settings")]
    [SerializeField, Range(.1f, 2)]  private float _minNodeDistance = 1;

    [Header("Some behavior settings")]
    [SerializeField, Range(1, 20)] private float _minGetAwayFromTargetDistance = 10;

    public float FallDetectionOriginDistance { get {return _fallDetectionOriginDistance; } }
    public float FallDetectionVerticalMinDistance {get {return _fallDetectionVerticalMinDistance; } }
    public float MaxJumpDistanceMultiplier { get {return _maxJumpDistanceMultiplier; } }
    public float CloserFloorHeightMinDistance { get {return _closerFloorHeightMinDistance; } }

    public float NodeMinDistanceTolerance { get { return _minNodeDistance; } }

    public float GetAwayFromTargetMinDistance { get { return _minGetAwayFromTargetDistance; } }
}