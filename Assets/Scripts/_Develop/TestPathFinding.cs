using System.Collections.Generic;
using UnityEngine;

public class TestPathFinding : MonoBehaviour
{
    public BaseCharacter _target;
    private BaseCharacter _testingCharacter;
    private BaseAIControls _controlScript;
    private AICharacterValues _aiValues;

    public List<Node> _path = new();
    

    [Header("Path configs")]
    public float _travelPointDistanceTolerance = 0.5f;
    public float _minAwayDistance = 10;


    [Header("Path results")]
    public Vector3 _currentDirection = Vector3.zero;
    public List<Node> _nodesToTravel = new();
    public int _travelIndex = 0;
    public bool _travelFinished = true;

    [Header("Jump stuff")]
    [Range(0.1f, 2)] public float _fallDetectionOriginMinDistance = 1;
    [Range(0.1f, 5)] public float _fallDetectionVerticalMinDistance = 1;
    [Range(.01f, .5f)] public float _maxJumpDistance = 0.3f;
    [Range(0.1f, 4)] public float _nextFloorHeightMinDistance = 1;

    public LayerMask _floorMask;
    public bool _fallDetected = false;
    public bool _floorToJumpDetected = false;

    private void Awake() 
    {
        _testingCharacter = GetComponent<BaseCharacter>();
        _controlScript = GetComponent<BaseAIControls>();
        _aiValues = GetComponent<AICharacterValues>();
    }

    public void MakeNewPathTowards()
    {
        _travelFinished = true;
        _path = PathfindingEvents.GetPath(_testingCharacter.transform, _target.transform);
    }

    public void MakeNewPathAway()
    {
        _travelFinished = true;
        _path = PathfindingEvents.GetRandomReachableNodesAwayFrom(this.gameObject, _target.gameObject, _minAwayDistance);
    }

    public void Walk(bool towards)
    {
        _travelFinished = true;

        if (_path.Count == 0) return;
        if (towards)
        {
            Debug.Log("Walking towards...");
            SetWayPoints(_path);
        }
        else
        {
            Debug.Log("Walking away...");
            SetWayPoints(_path);
        }

        if (_nodesToTravel.Count > 0) _travelFinished = false;        
    }

    public void SetWayPoints(List<Node> newPoints)
    {
        if (newPoints == null || newPoints.Count == 0)
        {
            _travelFinished = true;
            return;
        }

        _nodesToTravel = newPoints;
        // add target location at the end ?
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

        if (_travelFinished || _path.Count == 0) return;
        
        if (!_travelFinished && _nodesToTravel.Count > 0)
        {
            if (_aiValues.OnNodeJump)
            {
                Vector3 pos = _testingCharacter.CharacterPosition;
                Vector3 forwardFallDetect = pos + _testingCharacter.CharacterForward * _fallDetectionOriginMinDistance;
                Vector3 fallDetect = forwardFallDetect + Vector3.down * _fallDetectionVerticalMinDistance;
                Vector3 forwardJump = forwardFallDetect + _maxJumpDistance * _testingCharacter.CharacterData.JumpForce * _testingCharacter.CharacterForward;

                Debug.DrawLine(pos, forwardFallDetect, Color.red);
                Debug.DrawLine(forwardFallDetect, fallDetect, Color.yellow);
                Debug.DrawLine(forwardFallDetect, forwardJump, Color.blue);
                Debug.DrawLine(forwardJump, forwardJump + Vector3.down * _nextFloorHeightMinDistance, Color.green);
                Debug.DrawLine(forwardJump, forwardJump + Vector3.up * _nextFloorHeightMinDistance, Color.green);

                _fallDetected = !Physics.Raycast(forwardFallDetect, Vector3.down, _fallDetectionVerticalMinDistance, _floorMask);
                _floorToJumpDetected =  Physics.Raycast(forwardJump, Vector3.down, _nextFloorHeightMinDistance, _floorMask) ||
                                        Physics.Raycast(forwardJump + Vector3.up, Vector3.down, _nextFloorHeightMinDistance, _floorMask) ||
                                        Physics.Raycast(forwardJump + Vector3.down, Vector3.up, _nextFloorHeightMinDistance, _floorMask);

                if (_fallDetected && _floorToJumpDetected && !_travelFinished) _testingCharacter.Jump();
            }
        }

        var point = _nodesToTravel[_travelIndex];
        Vector3 direction = point.NodePosition - transform.position;
        direction.y = 0; // Remove height
        _currentDirection = direction;

        if (direction.magnitude < _travelPointDistanceTolerance)
        {
            if (_travelIndex + 1 < _nodesToTravel.Count) _travelIndex++;
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
