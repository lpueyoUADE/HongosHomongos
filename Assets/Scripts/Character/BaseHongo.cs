using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHongo : BaseCharacter, IHongo
{
    private void FixedUpdate()
    {
        if (!CharacterInControl) return;

        if (inputDir.y != 0) WeaponReference.transform.Rotate(Vector3.forward, inputDir.y);
        if (inputDir.x != 0) Move(new Vector3(inputDir.x, 0));
    }
}
