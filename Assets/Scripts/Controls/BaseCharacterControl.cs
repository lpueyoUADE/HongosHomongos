using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterControl : MonoBehaviour, IControlleable
{
    private BaseCharacter _character;
    public BaseCharacter Character => _character;

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
