using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IControlleable
{
    void InputMove(Vector3 dir);
    void InputAim(Vector3 dir);
}
