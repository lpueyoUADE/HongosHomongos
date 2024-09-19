using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : MonoBehaviour, IProjectile
{
    [Header("Projectile settings")]
    public float _baseSpeed = 0.25f;
    public float _baseLife = 5;
    public AudioClip _destroyedSound;

    // Values
    private Vector3 direction;
    private Rigidbody _rBody;
    private Coroutine _lifeTimer;

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
        this.direction = direction;
    }

    public void UpdateSpeedMultiplier(float speedMultiplier)
    {
        _rBody.AddForce(direction * _baseSpeed * speedMultiplier, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnDeath();
        Destroy(gameObject);
    }

    public void OnDeath()
    {
        if (_lifeTimer != null) StopCoroutine(_lifeTimer);
        PlaySoundEvents.PlaySound(transform.position, _destroyedSound, 1);
        GameTurnEvents.OnProjectileDeath?.Invoke();
    }

    IEnumerator ProjectileLife()
    {
        yield return new WaitForSeconds(_baseLife);
        OnDeath();
    }
}
