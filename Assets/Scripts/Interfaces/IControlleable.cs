using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControlleable
{
    void InputMove(Vector3 dir);
    void InputAim(Vector3 dir);
    BaseCharacter Character { get; }
    Vector3 Position { get; }
    Vector3 AimDirection { get; }
    Vector3 ProjectileOutPosition { get; }
}
