using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : MonoBehaviour, IProjectile
{
    [Header("Projectile settings")]
    public float _baseDamage = 2.2f;
    public float _baseSpeed = 0.25f;
    public float _baseLife = 5;
    public AudioClip _destroyedSound;

    // Values
    private Vector3 _direction;
    private Vector3 _lastPosition;
    private Rigidbody _rBody;
    private Coroutine _lifeTimer;
    private bool _alreadyDead;

    public GameObject Projectile => this.gameObject;

    private void Awake()
    {
        _rBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _lifeTimer = StartCoroutine(ProjectileLife());
    }

    public void UpdateDirection(Vector3 direction)
    {
        this._direction = direction;
    }

    public void UpdateSpeedMultiplier(float speedMultiplier)
    {
        _rBody.AddForce(_direction * _baseSpeed * speedMultiplier, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        _lastPosition = transform.position;
        collision.gameObject.TryGetComponent(out IDamageable character);
        if (character != null) DamageCharacter(character);

        OnDeath();
        Destroy(gameObject);
    }

    private void DamageCharacter(IDamageable objetive)
    {
        objetive.AnyDamage(_baseDamage);
    }

    public void OnDeath()
    {
        if (_alreadyDead) return;
        if (_lifeTimer != null) StopCoroutine(_lifeTimer);

        PlaySoundEvents.PlaySound?.Invoke(_lastPosition, _destroyedSound, 1);
        GameTurnEvents.OnProjectileDeath?.Invoke();
        _alreadyDead = true;
    }

    IEnumerator ProjectileLife()
    {
        yield return new WaitForSeconds(_baseLife);
        OnDeath();
    }
}
