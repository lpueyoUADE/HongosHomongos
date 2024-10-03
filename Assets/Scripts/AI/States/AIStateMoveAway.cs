using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateMoveAway<T> : State<T>
{
    private AIManager _controller;
    private List<Node> _path = new();
    private int _travelIndex = 0;
    private bool _travelFinished = true;
    private Vector3 _currentDirection = Vector3.zero;
    private BaseCharacter ControlledCharacter => _controller.CurrentControlledCharacter;
    private CharacterData ControlledCharacterData => ControlledCharacter.CharacterData;
    private AICharacterConfig ControlledCharacterAIData => (AICharacterConfig)ControlledCharacter.CharacterData;
    private AICharacterValues ControlledAIValues => ControlledCharacter.GetComponent<AICharacterValues>();

    // Jumping stuff
    private bool _fallDetected = false;
    private bool _floorToJumpDetected = false;

    public AIStateMoveAway(AIManager controller)
    {
        _controller = controller;
    }

    public override void Enter()
    {
        // When entering, generate a path away from the target
        SetWaypoints(PathfindingEvents.GetRandomReachableNodesAwayFrom(ControlledCharacter.gameObject, _controller.TargetTransform.gameObject, ControlledCharacterAIData.GetAwayFromTargetMinDistance));
        _controller.StartTimer(StatesEnum.MoveAway);
    }

    public override void Execute()
    {
        if (_travelFinished) return;
        NodeTravelling();
        DetectJumping();
    }

    public override void LateExecute()
    {
        if (_travelFinished) return;

        _controller.CurrentIControlleable.InputMove(_currentDirection.normalized);
    }

    public override void Sleep()
    {
        _travelFinished = true;
        _travelIndex = 0;
        _fallDetected = false;
        _floorToJumpDetected = false;

        _path.Clear();
    }

    // Travelling stuff

    private void SetWaypoints(List<Node> newPoints)
    {
        if (newPoints == null || newPoints.Count == 0) return;

        _path = newPoints;
        _travelFinished = false;
    }

    private void NodeTravelling()
    {

#if UNITY_EDITOR
        if (_path.Count > 0)
        {
            for (int i = 0; i < _path.Count; i++)
            {
                if (i == _path.Count - 1) break;
                Debug.DrawLine(_path[i].NodePosition, _path[i + 1].NodePosition, Color.red);
            }
        }
#endif

        var point = _path[_travelIndex];
        Vector3 direction = point.NodePosition - ControlledCharacter.CharacterPosition;
        direction.y = 0; // Remove height
        _currentDirection = direction;

        if (direction.magnitude < ControlledCharacterAIData.NodeMinDistanceTolerance)
        {
            if (_travelIndex + 1 < _path.Count) _travelIndex++;
            else _travelFinished = true;
        }
    }

    private void DetectJumping()
    {
        if (ControlledAIValues.OnNodeJump)
        {
            Vector3 charPos = _controller.CurrentControlledCharacter.CharacterPosition;
            Vector3 charForward = _controller.CurrentControlledCharacter.CharacterForward;
            Vector3 forwardFallDetect = charPos + charForward *  ControlledCharacterAIData.FallDetectionOriginDistance;
            Vector3 forwardJump = forwardFallDetect + ControlledCharacterAIData.MaxJumpDistanceMultiplier * ControlledCharacterData.JumpForce * charForward;

            LayerMask floorMask = ControlledCharacterData.FloorMask;
            float closerFloorMinHeight = ControlledCharacterAIData.CloserFloorHeightMinDistance;
            
#if UNITY_EDITOR
            Vector3 fallDetect = forwardFallDetect + Vector3.down * ControlledCharacterAIData.FallDamageMinDistance;

            Debug.DrawLine(charPos, forwardFallDetect, Color.red);
            Debug.DrawLine(forwardFallDetect, fallDetect, Color.yellow);
            Debug.DrawLine(forwardFallDetect, forwardJump, Color.blue);
            Debug.DrawLine(forwardJump, forwardJump + Vector3.down * closerFloorMinHeight, Color.green);
            Debug.DrawLine(forwardJump, forwardJump + Vector3.up * closerFloorMinHeight, Color.green);
#endif

            _fallDetected = !Physics.Raycast(forwardFallDetect, Vector3.down, ControlledCharacterAIData.FallDamageMinDistance, floorMask);
            _floorToJumpDetected =  Physics.Raycast(forwardJump, Vector3.up, closerFloorMinHeight, floorMask) ||
                                    Physics.Raycast(forwardJump, Vector3.down, closerFloorMinHeight, floorMask) ||
                                    Physics.Raycast(forwardJump + Vector3.up, Vector3.down, closerFloorMinHeight, floorMask) ||
                                    Physics.Raycast(forwardJump + Vector3.down, Vector3.up, closerFloorMinHeight, floorMask);

            if (_fallDetected && _floorToJumpDetected && !_travelFinished) ControlledCharacter.Jump();
        }
    }
}
