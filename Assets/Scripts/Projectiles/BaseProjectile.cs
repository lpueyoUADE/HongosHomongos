using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : MonoBehaviour, IProjectile
{
    private CharacterAbilityData _abilityData;
    private Vector3 _direction;
    private Vector3 _lastPosition;
    private Rigidbody _rBody;
    private Coroutine _lifeTimer;
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
        _lastPosition = transform.position;
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

    public virtual void UpdateData(CharacterAbilityData abilityData, Vector3 direction, float speedMultiplier, CapsuleCollider ownerCollider)
    {
        _abilityData = abilityData;
        _direction = direction;
        _rBody.AddForce(_direction * _abilityData.AbilityProjectileBaseSpeed * speedMultiplier, ForceMode.Impulse);

        IgnoreOwnerCollision(ownerCollider);
        _lifeTimer = StartCoroutine(ProjectileLife());
    }

    public virtual void DamageCharacter(IDamageable objetive)
    {
        objetive.AnyDamage(_abilityData.AbilityProjectileBaseDamage);
    }

    public virtual void OnCharacterHit(Collision character)
    {
        if (!_abilityData.AbilityResidualPrefab || _resSpawned) return;

        _resSpawned = true;
        transform.GetPositionAndRotation(out Vector3 spos, out Quaternion srot);        
        GameObject residual = Instantiate(_abilityData.AbilityResidualPrefab, spos, srot);
        residual.transform.SetParent(character.transform, true);
    }

    public virtual void OnWorldHit()
    {
        if (!_abilityData.AbilityResidualPrefab || _resSpawned) return;

        _resSpawned = true;
        Instantiate(_abilityData.AbilityResidualPrefab, transform.position, transform.rotation);
    }

    public virtual void OnDeath()
    {
        if (_alreadyDead) return;
        if (_lifeTimer != null) StopCoroutine(_lifeTimer);
        _alreadyDead = true;

        PlaySoundEvents.PlaySound?.Invoke(_lastPosition, _abilityData.AbilitDestroyedSound, 1);
        GameTurnEvents.OnProjectileDeath?.Invoke();
    }

    private void IgnoreOwnerCollision(CapsuleCollider ownerCollider)
    {
        TryGetComponent(out SphereCollider sphere);        
        if (sphere)
        {
            Physics.IgnoreCollision(sphere, ownerCollider, true);
            return;
        }

        TryGetComponent(out BoxCollider box);
        if (box)
        {
            Physics.IgnoreCollision(box, ownerCollider, true);
            return;
        }

        TryGetComponent(out CapsuleCollider capsule);
        if (capsule)
        {
            Physics.IgnoreCollision(capsule, ownerCollider, true);
            return;
        }
    }

    IEnumerator ProjectileLife()
    {
        yield return new WaitForSeconds(_abilityData.AbilityProjectileBaseLife);
        OnDeath();
    }
}
