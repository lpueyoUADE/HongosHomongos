using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBreakable : MonoBehaviour, IDamageable
{
    public List<AudioClip> _breakClips= new List<AudioClip>();
    public List<AudioClip> _hitClips = new List<AudioClip>();

    private AudioSource _audio;
    private float _startingZ;

    public bool IsDead => false;

    private float _delayBeforeAudio = 0.5f;
    private bool CanPlayAudio => _delayBeforeAudio <= 0;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _startingZ = transform.position.z;
    }

    private void Update()
    {
        if (_delayBeforeAudio > 0)
            _delayBeforeAudio -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        /*
        Vector3 position = transform.position;
        transform.position = new Vector3(position.x, position.y, _startingZ);
        */
    }

    public void AnyDamage(float amount)
    {
        OnBreak();
    }

    public void AnyDamage(int amount)
    {
        OnBreak();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_audio != null && !_audio.isPlaying && CanPlayAudio && _audio.enabled)
            _audio.PlayOneShot(_hitClips[Random.Range(0, _hitClips.Count)]);
    }

    public void OnBreak()
    {
        Destroy(gameObject);
    }
}
