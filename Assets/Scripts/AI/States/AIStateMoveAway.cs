using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateMoveAway<T> : State<T>
{
    private AIManager _controller;

    public AIStateMoveAway(AIManager controller)
    {
        _controller = controller;
    }

    public override void Enter()
    {
        _controller.StartTimer(StatesEnum.MoveAway);
    }

    public override void Execute()
    {
        Vector3 charPos = _controller.CurrentControlledCharacter.CharacterPosition;
        Vector3 charForward = _controller.CurrentControlledCharacter.CharacterForward;
        Vector3 charDown = -_controller.CurrentControlledCharacter.CharacterUp;

        Debug.DrawLine(charPos, charPos + charForward, Color.yellow);
        Debug.DrawLine(charPos + charForward, charPos + charForward + charDown * 5, Color.yellow);
    }

    public override void LateExecute()
    {
        _controller.CurrentIControlleable.InputMove(new Vector3(_controller.TargetDirection, 0));
    }
}
