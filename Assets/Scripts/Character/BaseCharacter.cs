using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter : MonoBehaviour, ICharacter, IDamageable
{
    [SerializeField] private float _maxLife = 10;
    [SerializeField] private float _speed = 3;

    // References
    [SerializeField] private GameObject _weaponReference;
    private Rigidbody _rBody;

    // Values
    private bool _inControl = false;

    public GameObject WeaponReference => _weaponReference;
    public Vector3 inputDir => new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    virtual public bool IsDead => _maxLife <= 0;
    public bool CharacterInControl => _inControl;

    private void Awake()
    {
        _rBody = GetComponent<Rigidbody>();
    }

    virtual public void AnyDamage(float amount)
    {
        
    }

    virtual public void AnyDamage(int amount)
    {
        
    }

    virtual public void InControl(bool isInControl = false)
    {
        _inControl = isInControl;
    }

    virtual public void Move(Vector3 direction, float speed, ForceMode mode = ForceMode.Impulse)
    {
        _rBody.AddForce(direction * speed, mode);
    }

    virtual public void Move(Vector3 direction, ForceMode mode = ForceMode.Impulse)
    {
        _rBody.AddForce(direction * _speed / 2, mode);
    }
}
