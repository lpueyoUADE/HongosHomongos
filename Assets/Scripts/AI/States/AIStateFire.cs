using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateFire<T> : State<T>
{
    private AIManager _controller;

    public AIStateFire(AIManager controller)
    {
        _controller = controller;
    }

    public override void Enter()
    {
        _controller.CurrentControlledCharacter.ChargeWeapon();
    }

    public override void Sleep()
    {
        _controller.ForceStopTurn();
        _controller.CurrentControlledCharacter.ChargeWeaponStop();
    }
}
