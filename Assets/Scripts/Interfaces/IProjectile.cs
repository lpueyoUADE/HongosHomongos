using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile
{
    void UpdateDirection(Vector3 direction);
    void UpdateSpeedMultiplier(float speedMultiplier);
    void OnDeath();

    public GameObject Projectile { get; }
}
