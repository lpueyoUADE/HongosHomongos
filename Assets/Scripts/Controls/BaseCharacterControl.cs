using UnityEngine;
using UnityEngine.AI;

public class BaseCharacterControl : MonoBehaviour, IControlleable
{
    public float _aimSpeed = 2;

    private BaseCharacter _character;
    private NavMeshAgent _navAgent;

    public BaseCharacter Character => _character;
    public Vector3 Position => transform.position;
    public Vector3 AimDirection => Character.AimingDirection;
    public Vector3 ProjectileOutPosition => Character.ProjectileOutPosition;
    public NavMeshAgent NavAgent => _navAgent;

    private void Awake()
    {
        _character = GetComponent<BaseCharacter>();

        TryGetComponent(out NavMeshAgent nmAgent);
        if (nmAgent)
        {
            _navAgent = nmAgent;
            _navAgent.updateRotation = false;
            _navAgent.speed = Character.CharacterData.Speed;
        }
    }

    public virtual void InputAim(Vector3 dir)
    {
        Character.Aim(dir);
    }

    public virtual void InputMove(Vector3 dir)
    {
        Character.Move(dir);
    }

    public virtual void InputSelectAbility(int selection)
    {
        Character.ChangeAbility(selection);
    }

    public void NavAgentSetDestination(Vector3 newPath)
    {
        _navAgent.isStopped = false;
        _navAgent.SetDestination(newPath);
    }

    public void NavAgentForceStop()
    {
        _navAgent.ResetPath();
        _navAgent.isStopped = true;
    }
}
