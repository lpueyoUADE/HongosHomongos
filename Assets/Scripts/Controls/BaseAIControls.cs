using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAIControls : BaseCharacterControl
{

    private void OnEnable()
    {
        AIManagerEvents.OnCharacterControlUpdate?.Invoke(this, true);
    }

    private void OnDisable()
    {
        AIManagerEvents.OnCharacterControlUpdate?.Invoke(this, false);
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {

    }
}
