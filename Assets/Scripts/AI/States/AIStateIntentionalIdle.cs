using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateIntentionalIdle<T> : State<T>
{
    private AIManager _controller;

    public AIStateIntentionalIdle(AIManager controller)
    {
        _controller = controller;
    }

    public override void Enter()
    {
        _controller.StartTimer(StatesEnum.IntentionalIdle);
    }
}
