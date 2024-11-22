using UnityEngine;
using UnityEngine.AI;

public interface IControlleable
{
    void InputMove(Vector3 dir);
    void InputAim(Vector3 dir);
    void NavAgentSetDestination(Vector3 newPath);
    void NavAgentForceStop();
    BaseCharacter Character { get; }
    NavMeshAgent NavAgent { get; }
}
