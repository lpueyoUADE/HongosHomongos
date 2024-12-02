public class AIStateFire<T> : State<T>
{
    private AIManager _controller;

    public AIStateFire(AIManager controller)
    {
        _controller = controller;
    }

    public override void Enter()
    {
        _controller.CurrentControlledCharacter.CharacterAnimation.SetBool("IsAiming", true);
        _controller.CurrentControlledCharacter.ChargeAbility();
    }

    public override void Sleep()
    {
        _controller.CurrentControlledCharacter.CharacterAnimation.SetBool("IsAiming", false);
        _controller.ForceStopTurn();
        _controller.CurrentControlledCharacter.ChargeAbilityStop();
    }
}
