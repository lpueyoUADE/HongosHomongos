using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter
{
    void Move(Vector3 direction, float speed, ForceMode mode = ForceMode.Impulse);
    void Move(Vector3 direction, ForceMode mode = ForceMode.Impulse);
    void InControl(bool isInControl = false);
}
