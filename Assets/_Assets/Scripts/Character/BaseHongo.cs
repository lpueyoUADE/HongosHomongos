using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHongo : MonoBehaviour, IHongo
{
    [SerializeField] private float _speed = 3;

    private Rigidbody _rBody;

    private void Awake()
    {
        _rBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {

    }

    public void Move(Vector3 direction, float speed, ForceMode mode = ForceMode.Impulse)
    {
        _rBody.AddForce(direction * speed, mode);
    }

    public void Move(Vector3 direction, ForceMode mode = ForceMode.Impulse)
    {
        _rBody.AddForce(direction * _speed, mode);
    }
}
