using UnityEngine;

public class AIStateSwapTarget<T> : State<T>
{
    private AIManager _controller;

    public AIStateSwapTarget(AIManager controller)
    {
        _controller = controller;
    }

    public override void Enter()
    {
        _controller.SwapTarget();
    }
}
