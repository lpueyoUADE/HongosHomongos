using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHongo
{
    void Move(Vector3 direction, float speed, ForceMode mode = ForceMode.Impulse);
    void Move(Vector3 direction, ForceMode mode = ForceMode.Impulse);
}
