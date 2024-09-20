using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseCharacter : MonoBehaviour, ICharacter, IDamageable
{
    [Header("Character settings")]
    [SerializeField] private float _maxLife = 10;
    [SerializeField] private float _speed = 3;

    [Header("Projectile settings")]
    [SerializeField] private BaseProjectile _spawnedProjectile;
    [SerializeField] private GameObject _weaponReference;
    [SerializeField] private GameObject _projectileOutReference;
    [SerializeField] private AudioClip _projectileSound;

    [Header("UI Stuff")]
    [SerializeField] private GameObject _uiObjects;
    [SerializeField] private TextMeshProUGUI _nameTextRef;
    [SerializeField] private Image _lifeBar;

    // Values
    private float _initialLife;
    private AudioSource _audio;
    private BaseCharacterControl _controlsScript;
    private CapsuleCollider _baseCollider;
    private Rigidbody _rBody;
    private bool _inControl = false;

    public GameObject WeaponReference => _weaponReference;
    virtual public bool IsDead => _maxLife <= 0;
    public bool CharacterInControl => _inControl;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _rBody = GetComponent<Rigidbody>();
        _controlsScript = GetComponent<BaseCharacterControl>();
        _baseCollider = GetComponent<CapsuleCollider>();

        _initialLife = _maxLife;
    }

    protected virtual void Update()
    {
        if (!CharacterInControl) return;
    }

    virtual public void AnyDamage(float amount)
    {
        _maxLife -= amount;
        OnDamage();
    }

    virtual public void AnyDamage(int amount)
    {
        _maxLife -= amount;
        OnDamage();
    }

    virtual public void OnDamage()
    {
        _lifeBar.fillAmount = (_maxLife / _initialLife) * 1;
        if (CharacterInControl) GameTurnEvents.OnTurnEnd?.Invoke(null);

        if (IsDead) OnDeath();
    }

    virtual public void OnDeath()
    {
        _uiObjects.SetActive(false);
        GameManagerEvents.OnCharacterDeath?.Invoke(this);
    }

    virtual public void InControl(bool isInControl = false)
    {
        _inControl = isInControl;
        _controlsScript.enabled = isInControl;

        if (!isInControl) InGameUIEvents.OnChargingWeaponBar?.Invoke(false);
    }

    virtual public void Move(Vector3 direction, float speed, ForceMode mode = ForceMode.Impulse)
    {
        _rBody.AddForce(direction * speed, mode);
    }

    virtual public void Move(Vector3 direction, ForceMode mode = ForceMode.Impulse)
    {
        _rBody.AddForce(direction * _speed / 2, mode);
    }

    virtual public void Aim(Vector3 direction)
    {
        WeaponReference.transform.Rotate(Vector3.forward, direction.y);
    }

    virtual public void ChargeWeapon()
    {
        InGameUIEvents.OnChargingWeaponBar?.Invoke(true);
    }

    virtual public void ChargeWeaponStop()
    {
        float force = InGameUIEvents.GetChargerBarIntensity();

        // If we are in control then OnTurnEnd True will make camera follow the spawned projectile.
        if (CharacterInControl)
        {
            // WIP - needs to modify to make self-damage possible
            var spawnedProjectile = Instantiate(_spawnedProjectile, _weaponReference.transform.position, new Quaternion());
            SphereCollider collider = spawnedProjectile.GetComponent<SphereCollider>();
            Physics.IgnoreCollision(_baseCollider, collider, true);

            IProjectile proData = spawnedProjectile.GetComponent<IProjectile>();
            proData?.UpdateDirection(_projectileOutReference.transform.right);
            proData?.UpdateSpeedMultiplier(force);

            _audio.PlayOneShot(_projectileSound);
            GameTurnEvents.OnTurnEnd?.Invoke(proData);
        }

        else
            GameTurnEvents.OnTurnEnd?.Invoke(null);

        InGameUIEvents.OnChargingWeaponBar?.Invoke(false);
    }

    public void UpdateName(string newName)
    {
        _nameTextRef.text = newName;
    }
}