using UnityEngine;

public class ProjectileFireball : BaseProjectile
{
    public LayerMask _affectedLayer;

    protected override void OnCollisionEnter(Collision collision)
    {
        Collider[] victims = new Collider[20];
        Physics.OverlapSphereNonAlloc(transform.position, AreaAbilityData.AbilityAreaRadius, victims, _affectedLayer);

        foreach (Collider item in victims)
        {
            if (item == null) continue;

            item.gameObject.TryGetComponent(out IDamageable damageable); 
            if (damageable == null) continue;

            if (AreaAbilityData.AbilityAreaDecayDamageByDistance)
            {
                float distance = (transform.position - item.transform.position).magnitude;
                float ratio = 1 - (distance / AreaAbilityData.AbilityAreaRadius);
                damageable.AnyDamage(AbilityData.AbilityProjectileBaseDamage * ratio);
            }

            else damageable.AnyDamage(AbilityData.AbilityProjectileBaseDamage);
        }

        OnDeath();
        Destroy(gameObject);
    }
}