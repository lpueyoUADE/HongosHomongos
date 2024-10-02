using System.Collections.Generic;
using UnityEngine;

public class TestPathFinding : MonoBehaviour
{
    public BaseCharacter _target;
    private BaseCharacter _testingCharacter;
    private BaseAIControls _controlScript;

    public BaseCharacter AICharacter => _testingCharacter;


    public List<Node> _path = new();
    

    [Header("Path configs")]
    public float _travelPointDistanceTolerance = 0.5f;
    public float _minAwayDistance = 10;


    [Header("Path results")]
    public Vector3 _currentDirection = Vector3.zero;
    public List<Vector3> _positionsToTravel = new();
    public int _travelIndex = 0;
    public bool _travelFinished = true;

    [Header("Jump stuff")]
    public float _floorDetectionMinDistanceDivider = 1.5f;
    [Tooltip("How hight/low a platform/floor can be to be detected")] public float _floorDetectionHeightDistanceDivisor = 3;
    [Tooltip("Divide jump force by this to get an estimate of how far the jump can reach in a straight line.")] public float _jumpDistanceDivider = 3.25f;
    [Tooltip("How high a platform can be to this character to be able to jump to it.")] public float _platformHeightDivider = 12;
    public LayerMask _floorMask;
    public bool _fallDetected = false;
    public bool _floorToJumpDetected = false;

    private void Awake() 
    {
        _testingCharacter = GetComponent<BaseCharacter>();
        _controlScript = GetComponent<BaseAIControls>();
    }

    public void MakeNewPath()
    {
        _path = PathfindingEvents.GetPath(_testingCharacter.transform, _target.transform);
    }

    public void Walk(bool towards)
    {
        if (towards)
        {
            Debug.Log("Walking towards...");
            SetWayPoints(_path);
            return;
        }

    }

    public void SetWayPoints(List<Vector3> newPoints)
    {
        if (newPoints.Count == 0)
        {
            _travelFinished = true;
            return;
        }

        _travelIndex = 0;
        _positionsToTravel = newPoints;
        // fix height required?

        _travelFinished = false;
    }

    public void SetWayPoints(List<Node> newPoints)
    {
        if (newPoints == null || newPoints.Count == 0)
        {
            _travelFinished = true;
            return;
        }

        var list = new List<Vector3>();

        for (int i = 0; i < newPoints.Count; i++) list.Add(newPoints[i].NodePosition);
        // add target location at the end ?
        SetWayPoints(list);
    }


    private void Update() 
    {
        if (_path.Count > 0)
        {
            for (int i = 0; i < _path.Count; i++)
            {
                if (i == _path.Count - 1) break;
                Debug.DrawLine(_path[i].NodePosition, _path[i + 1].NodePosition, Color.red);
            }
        }


        Vector3 charPos = _testingCharacter.CharacterPosition;
        Vector3 charForward = _testingCharacter.CharacterForward / _floorDetectionMinDistanceDivider;
        Vector3 charDown = -_testingCharacter.CharacterUp;

        float jumpDistace = _testingCharacter.CharacterData.JumpForce / _jumpDistanceDivider;
        float jumpDistanceToAnotherPlatform = _testingCharacter.CharacterData.JumpForce / _platformHeightDivider;
        float fallDistance = _testingCharacter.CharacterData.FallDamageMinDistance;        

        Vector3 originLine = _testingCharacter.CharacterPosition + -charForward;
        Vector3 originLineJumpDistance = _testingCharacter.CharacterPosition + -(charForward * jumpDistace);

        // Draw some lines...
        Debug.DrawLine(originLine, originLine + charDown * 2, Color.blue);
        Debug.DrawLine(originLineJumpDistance + _testingCharacter.CharacterUp * jumpDistanceToAnotherPlatform, originLineJumpDistance + charDown * 2, Color.yellow);

        _fallDetected = !Physics.Raycast(originLine, charDown, _testingCharacter.CharacterData.FallDamageMinDistance, _floorMask);
        _floorToJumpDetected = Physics.Raycast(originLineJumpDistance + _testingCharacter.CharacterUp * jumpDistanceToAnotherPlatform, charDown, _testingCharacter.CharacterData.FallDamageMinDistance / _floorDetectionHeightDistanceDivisor, _floorMask)
        || Physics.Raycast(originLineJumpDistance - _testingCharacter.CharacterUp * jumpDistanceToAnotherPlatform, -charDown, _testingCharacter.CharacterData.FallDamageMinDistance / _floorDetectionHeightDistanceDivisor, _floorMask);

        if (_fallDetected && _floorToJumpDetected && !_travelFinished) _testingCharacter.Jump();
        if (_travelFinished) return;

        var point = _positionsToTravel[_travelIndex];
        Vector3 direction = point - transform.position;
        direction.y = 0; // Remove height
        _currentDirection = direction;

        if (direction.magnitude < _travelPointDistanceTolerance)
        {
            if (_travelIndex + 1 < _positionsToTravel.Count) _travelIndex++;
            else
            {
                _travelFinished = true;
                return;
            }
        }
    }

    private void FixedUpdate() 
    {
        if (_currentDirection == Vector3.zero || _travelFinished)  return;

        _controlScript.InputMove(_currentDirection.normalized);
    }
}
