using UnityEngine;

public class AIStateAim<T> : State<T>
{
    private AIManager _controller;
    bool test;

    public AIStateAim(AIManager controller)
    {
        _controller = controller;
    }

    public override void Enter()
    {
        _controller.StartTimer(StatesEnum.Aim);
        test = Random.Range(0, 50) >= 25;
    }

    public override void Execute()
    {
        Vector3 charPos = _controller.CurrentControlledCharacter.CharacterPosition;

        Debug.DrawLine(charPos, _controller.TargetPosition, Color.cyan);
        Debug.DrawLine(_controller.CurrentControlledCharacter.ProjectileOutPosition, charPos + _controller.CurrentControlledCharacter.AimingDirection * 50, Color.red);
    }

    public override void LateExecute()
    {
        if (test) _controller.CurrentIControlleable.InputAim(new Vector3(0, -0.25f));
        else _controller.CurrentIControlleable.InputAim(new Vector3(0, 0.25f));
    }

    public override void Sleep()
    {
        test = Random.Range(0, 50) >= 25;
        _controller.StartTimer(StatesEnum.Fire);
    }
}
