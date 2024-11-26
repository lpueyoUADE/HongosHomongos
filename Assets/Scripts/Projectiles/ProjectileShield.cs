using UnityEngine;

public class ProjectileShield : BaseProjectile
{
    private IAbilities _hitCharacter;

    protected override void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.TryGetComponent(out IAbilities character);
        if (character != null) _hitCharacter = character;

        base.OnCollisionEnter(collision);
    }

    public override void DamageCharacter(IDamageable objetive)
    {
        _hitCharacter?.OnAbilityShield(ShieldAbilityData.ShieldTurnsDuration, AbilityData.AbilityResidualPrefab, ShieldAbilityData.ShieldDamageReductionPercent);
    }

    
    public override void OnCharacterHit(Collision character)
    {
        return;
    }

    public override void OnWorldHit()
    {
        return;
    }
}
