using UnityEngine;

public class ProjectileHeal : BaseProjectile
{
    public GameObject victimsParticles;
    public LayerMask _affectedLayer;

    protected override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == OwnerObject && IgnoreOwner) return;
        
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

                Instantiate(victimsParticles, item.transform);
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

# if UNITY_EDITOR
    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, AreaAbilityData.AbilityAreaRadius);
    }
# endif
}
