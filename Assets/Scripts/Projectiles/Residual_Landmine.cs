using UnityEngine;

public class Residual_Landmine : MonoBehaviour
{
    private float _damage = 1;
    private float _explosionRadius = 1;
    private bool _damageDecaysOnDistance = true;
    [SerializeField] private int _triggerTimes = 1;
    public GameObject explosionParticles;
    public LayerMask affectedLayer;

    public void UpdateTrapData(float newDamage, float newExplosionRadius, bool newDmgDecaysOnDistance)
    {
        _damage = newDamage;
        _explosionRadius = newExplosionRadius;
        _damageDecaysOnDistance = newDmgDecaysOnDistance;
    }

    public virtual void OnTriggerEnter(Collider other) 
    {
        Debug.Log(other.name);
        Collider[] victims = new Collider[20];
        Physics.OverlapSphereNonAlloc(transform.position, _explosionRadius, victims, affectedLayer);

        foreach (Collider item in victims)
        {
            if (item == null) continue;

            item.gameObject.TryGetComponent(out IDamageable damageable); 
            if (damageable == null) continue;

            if (_damageDecaysOnDistance)
            {
                float distance = (transform.position - item.transform.position).magnitude;
                float ratio = 1 - (distance / _explosionRadius);
                damageable.AnyDamage(_damage * ratio);
            }

            else damageable.AnyDamage(_damage);
        }

        _triggerTimes--;
        Instantiate(explosionParticles, transform.position, new Quaternion());
        if (_triggerTimes <= 0) Destroy(gameObject);
    }

# if UNITY_EDITOR
    void OnDrawGizmos() 
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
# endif
}
