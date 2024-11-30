using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBerserk : BaseProjectile
{
    [SerializeField, Range(1.01f, 3)] private float _damageReceivedMultiplier = 1.8f;
    [SerializeField] private int _turnsDuration = 5;
    [SerializeField] private GameObject _weakenedParticles = null;

    void Start()
    {
        if (OwnerCollider == null) return;

        OwnerCollider.gameObject.TryGetComponent(out IAbilities abilitiesEffect);
        abilitiesEffect?.OnWeakened(_turnsDuration, _damageReceivedMultiplier, _weakenedParticles);
    }
}
