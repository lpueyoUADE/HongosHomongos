using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProjectile : MonoBehaviour, IProjectile
{
    public float baseSpeed = 0.25f;
    public Vector3 direction;

    private Rigidbody _rBody;

    private void Awake()
    {
        _rBody = GetComponent<Rigidbody>();
    }

    public void UpdateDirection(Vector3 direction)
    {
        this.direction = direction;
    }

    public void UpdateSpeedMultiplier(float speedMultiplier)
    {
        _rBody.AddForce(direction * baseSpeed * speedMultiplier, ForceMode.Impulse);
    }
}
