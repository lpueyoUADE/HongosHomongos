using UnityEngine;
using UnityEngine.AI;

public class BaseSapo : BaseCharacter, ISapo
{
    private NavMeshAgent _navAgent;

    private void Start()
    {
        _navAgent = GetComponent<NavMeshAgent>();
        transform.rotation = new Quaternion(0, 90, 0, 0);
    }

    public override void OnDeath()
    {
        base.OnDeath();
        Destroy(_navAgent);
        CharacterRigidbody.constraints = RigidbodyConstraints.None;
        CharacterRigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!IsDead && _navAgent != null) UpdateNavLinkRotation();
    }

    private void UpdateNavLinkRotation()
    {
        float xVel = _navAgent.velocity.x;
        if (xVel < _minVelocityToRotate && xVel < 0) _bodyMesh.transform.rotation = new Quaternion(0, 180, 0, 0);
        if (xVel > -_minVelocityToRotate && xVel > 0)  _bodyMesh.transform.rotation = new Quaternion(0, 0, 0, 0);
    }
}
