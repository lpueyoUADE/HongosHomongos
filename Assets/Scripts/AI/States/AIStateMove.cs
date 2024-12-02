using UnityEngine;
using UnityEngine.AI;

public class AIStateMove<T> : State<T>
{
    private AIManager _controller;
    private Vector3 _destination;

    public AIStateMove(AIManager controller)
    {
        _controller = controller;
    }

    public override void Enter()
    {
        if (!_controller.IsAITurn) return;

        _controller.StartTimer(StatesEnum.Move);

        if (_destination == Vector3.zero)
        {
            _destination = GetRandomPoint(25);            
            _controller.CurrentIControlleable.NavAgentSetDestination(_destination);

# if UNITY_EDITOR
            TestDebugBox.OnUpdateDebugBoxText?.Invoke($"AI destination = {_destination}");
# endif
        }

        _controller.CurrentControlledCharacter.CharacterAnimation.SetBool("IsMoving", true);
    }

    public override void LateExecute()
    {
        if (Vector3.Distance(_controller.CurrentControlledCharacter.CharacterPosition, _destination) < 2.5f)
            AIManagerEvents.OnActionFinished?.Invoke(StatesEnum.Move);
    }

    public override void Sleep()
    {
        _controller.CurrentControlledCharacter.CharacterAnimation.SetBool("IsMoving", false);
        _controller.CurrentIControlleable.NavAgentForceStop();
        _destination = Vector3.zero;
    }

    private Vector3 GetRandomPoint(float radius)
    {
        Vector3 randPos = Random.insideUnitSphere * radius;
        randPos += _controller.CurrentControlledCharacter.CharacterPosition;
        randPos.z = 0;
        Vector3 finalPos = Vector3.zero;

        if (NavMesh.SamplePosition(randPos, out NavMeshHit hit, radius, 1)) finalPos = hit.position;
        return finalPos;
    }
}
