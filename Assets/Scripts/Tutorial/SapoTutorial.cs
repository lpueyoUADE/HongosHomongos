using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SapoTutorial : BaseCharacter
{
    bool DamageReached => damageReceived >= neededDmg;
    public bool canDie = false;

    public float neededDmg = 100;
    public float damageReceived = 0;

    public override void AnyDamage(float amount)
    {
        damageReceived += amount;
        NotifyEnoughDamage();
        
        if (canDie) base.AnyDamage(amount);
    }

    public override void AnyDamage(int amount)
    {
        damageReceived += amount;
        NotifyEnoughDamage();
        
        if (canDie) base.AnyDamage(amount);
    }

    void NotifyEnoughDamage()
    {
        if (DamageReached) TutorialControls.OnEnemyDamaged?.Invoke();
    }
}
