using UnityEngine;

public class BaseCharacterControl : MonoBehaviour, IControlleable
{
    public float _aimSpeed = 2;

    private BaseCharacter _character;
    public BaseCharacter Character => _character;

    public Vector3 Position => transform.position;

    public Vector3 AimDirection => Character.AimingDirection;

    public Vector3 ProjectileOutPosition => Character.ProjectileOutPosition;

    private void Awake()
    {
        _character= GetComponent<BaseCharacter>();
    }

    public virtual void InputAim(Vector3 dir)
    {
        Character.Aim(dir);
    }

    public virtual void InputMove(Vector3 dir)
    {
        Character.Move(dir);
    }
}
