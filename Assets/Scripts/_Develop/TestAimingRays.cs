using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAimingRays : MonoBehaviour
{
    public BaseProjectile _projectile;
    public AudioClip _clip;
    public float _delay = 1;
    [Range(0.1f, 1)] public float _power = 0.5f;

    Coroutine timer;
    AudioSource _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
    }

    public void ResetTimer()
    {
        var spawnedProjectile = Instantiate(_projectile, transform.position, new Quaternion());
        IProjectile proData = spawnedProjectile.GetComponent<IProjectile>();
        proData?.UpdateDirection(transform.right);
        proData?.UpdateSpeedMultiplier(_power);
        _audio.PlayOneShot(_clip);

        timer = StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(_delay);
        ResetTimer();
    }
}
