using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateTest<T> : State<T>
{
    public AIStateTest() { }

    public override void Enter()
    {
#if UNITY_EDITOR
        TestAIStates.OnUpdateAIDebug?.Invoke($"Entering state: TEST - actions tree may have failed.");
# endif
    }
}