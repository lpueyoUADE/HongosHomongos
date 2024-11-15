using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateIdle<T> : State<T>
{
    private AIManager _controller;

    public AIStateIdle(AIManager controller)
    {
        _controller = controller;
    }

    public override void Enter()
    {
#if UNITY_EDITOR
        TestDebugBox.OnUpdateDebugBoxText?.Invoke($"Idle - waiting for turn.");
# endif
    }
}
