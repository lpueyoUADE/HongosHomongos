using UnityEngine;

public class ProjectileHeal : BaseProjectile
{
    public LayerMask _affectedLayer;

    protected override void OnCollisionEnter(Collision collision)
    {
        Collider[] victims = new Collider[20];
        Physics.OverlapSphereNonAlloc(transform.position, AreaAbilityData.AbilityAreaRadius, victims, _affectedLayer);

        foreach (Collider item in victims)
        {
            item.TryGetComponent(out IAbilities ability);
            if (ability == null) continue;

            if (AreaAbilityData.AbilityAreaDecayDamageByDistance) ability.OnAbilityHeal(AbilityData.AbilityProjectileBaseDamage * (1 - Vector3.Distance(transform.position, item.transform.position)));
            else ability.OnAbilityHeal(AbilityData.AbilityProjectileBaseDamage);
        }
    }
}
