using System.Collections;
using UnityEngine;

public class BaseProjectile : MonoBehaviour, IProjectile
{
    private CharacterAbilityData _abilityData;
    private Vector3 _direction;
    private Rigidbody _rBody;
    private Coroutine _lifeTimer;
    private GameObject _owner;
    private CapsuleCollider _ownerCollider;

    public Transform _projectileMesh;
    public bool _alreadyDead;
    public bool _resSpawned = false;
    public GameObject Projectile => gameObject;
    public CharacterAbilityData AbilityData => _abilityData;
    public ShieldAbilityData ShieldAbilityData => (ShieldAbilityData)_abilityData;
    public AreaAbilityData AreaAbilityData => (AreaAbilityData)_abilityData;
    
    private void Awake()
    {
        _rBody = GetComponent<Rigidbody>();
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {


        collision.gameObject.TryGetComponent(out IDamageable character);
        if (character != null)
        {
            DamageCharacter(character);
            OnCharacterHit(collision);
        }

        else OnWorldHit();

        OnDeath();        
        Destroy(gameObject);
    }

    protected virtual void OnCollisionExit(Collision other) 
    {
        if (other.gameObject == _owner) IgnoreOwnerCollision(false);
    }

    public virtual void UpdateData(CharacterAbilityData abilityData, Vector3 direction, float speedMultiplier, GameObject owner, CapsuleCollider ownerCollider)
    {
        _abilityData = abilityData;
        _direction = direction;
        _owner = owner;
        _ownerCollider = ownerCollider;
        IgnoreOwnerCollision();

        _lifeTimer = StartCoroutine(ProjectileLife());
        _rBody.AddForce(_direction * _abilityData.AbilityProjectileBaseSpeed * speedMultiplier, ForceMode.Impulse);
    }

    public virtual void DamageCharacter(IDamageable objetive)
    {
        objetive.AnyDamage(_abilityData.AbilityProjectileBaseDamage);
    }

    public virtual void OnCharacterHit(Collision character)
    {
        if (!_abilityData.AbilityResidualPrefab || _resSpawned) return;

        _resSpawned = true;
        GameObject res = Instantiate(_abilityData.AbilityResidualPrefab, transform.position, transform.rotation);
        if (_projectileMesh != null) res.transform.SetPositionAndRotation(_projectileMesh.position, _projectileMesh.rotation);
        res.transform.SetParent(character.transform, true);
    }

    public virtual void OnWorldHit()
    {
        if (!_abilityData.AbilityResidualPrefab || _resSpawned) return;

        _resSpawned = true;
        GameObject res = Instantiate(_abilityData.AbilityResidualPrefab, transform.position, transform.rotation);
        if (_projectileMesh != null) res.transform.SetPositionAndRotation(_projectileMesh.position, _projectileMesh.rotation);
    }

    public virtual void OnDeath()
    {
        if (_alreadyDead) return;
        if (_lifeTimer != null) StopCoroutine(_lifeTimer);
        _alreadyDead = true;

        PlaySoundEvents.PlaySound?.Invoke(transform.position, _abilityData.AbilitDestroyedSound, 1);
        GameTurnEvents.OnProjectileDeath?.Invoke();
    }

    private void IgnoreOwnerCollision(bool ignore = true)
    {
        TryGetComponent(out SphereCollider sphere);        
        if (sphere)
        {
            Physics.IgnoreCollision(sphere, _ownerCollider, ignore);
            return;
        }

        TryGetComponent(out BoxCollider box);
        if (box)
        {
            Physics.IgnoreCollision(box, _ownerCollider, ignore);
            return;
        }

        TryGetComponent(out CapsuleCollider capsule);
        if (capsule)
        {
            Physics.IgnoreCollision(capsule, _ownerCollider, ignore);
            return;
        }
    }

    IEnumerator ProjectileLife()
    {
        yield return new WaitForSeconds(_abilityData.AbilityProjectileBaseLife);
        OnDeath();
    }
}
