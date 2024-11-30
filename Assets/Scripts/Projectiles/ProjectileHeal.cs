using UnityEngine;

public class ProjectileHeal : BaseProjectile
{
    public GameObject victimsParticles;
    public LayerMask _affectedLayer;

    protected override void OnCollisionEnter(Collision collision)
    {
        Collider[] victims = new Collider[20];
        Physics.OverlapSphereNonAlloc(transform.position, AreaAbilityData.AbilityAreaRadius, victims, _affectedLayer);

        foreach (Collider item in victims)
        {
             if (item == null) continue;

            item.TryGetComponent(out IAbilities ability);
            if (ability == null) continue;

            if (AreaAbilityData.AbilityAreaDecayDamageByDistance) 
            {
                float distance = (transform.position - item.transform.position).magnitude;
                float ratio = 1 - (distance / AreaAbilityData.AbilityAreaRadius);
                ability.OnAbilityHeal(AbilityData.AbilityProjectileBaseDamage * ratio);
            }

            else 
            {
                Instantiate(AbilityData.AbilityResidualPrefab, item.transform);
                ability.OnAbilityHeal(AbilityData.AbilityProjectileBaseDamage);
            }
        }

        OnDeath();
        OnWorldHit();
        Destroy(gameObject);
    }
}
