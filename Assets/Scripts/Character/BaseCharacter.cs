using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseCharacter : MonoBehaviour, ICharacter, IDamageable
{
    [Header("Character settings")]
    [SerializeField] private float _life = 10;
    [SerializeField, Range(0.1f, 2)] private float _speed = 0.65f;
    [SerializeField, Range(0, 20)] private float _jumpForce = 12;

    [Header("Falling settings")]
    [SerializeField, Range(0, 1)] private float _fallSpeedModifier = 0.45f;
    [SerializeField, Range(25, 50)] private float _fallMaxSpeed = 35;
    [SerializeField, Range(0.5f, 10)] private float _fallDamageMinDistance = 6;
    [SerializeField, Range(0.1f, 10)] private float _fallDamageMultiplier = 4;
    [SerializeField, Range(0.1f, 0.5f)] private float _fallMinVelocityTolerance = 0.25f;

    [Header("Projectile settings")]
    [SerializeField] private BaseProjectile _spawnedProjectile;
    [SerializeField] private GameObject _weaponReference;
    [SerializeField] private GameObject _projectileOutReference;
    [SerializeField] private AudioClip _projectileSound;

    [Header("UI Stuff")]
    [SerializeField] private GameObject _uiObjects;
    [SerializeField] private TextMeshProUGUI _nameTextRef;
    [SerializeField] private Image _lifeBar;

    [Header("Sounds")]
    [SerializeField] private AudioClip _deathSound;
    [SerializeField] private AudioClip _fallDamageSound;
    [SerializeField] private AudioClip _jumpSound;

    [Header("Status")]
    public bool _isInAir;
    public bool _isFalling;
    [Tooltip("Not used - only for show.")] public float _lastFallDistanceWithDamage;

    // Values
    private bool _alreadyDead;
    private float _initialLife;
    private AudioSource _audio;
    private BaseCharacterControl _controlsScript;
    private CapsuleCollider _baseCollider;
    private Rigidbody _rBody;
    private bool _inControl = false;
    private float _fallStartingY;
    private float _fallDistance;

    public GameObject WeaponReference => _weaponReference;
    virtual public bool IsDead => _life <= 0;
    public bool CanJump => !_isInAir && !_isFalling;
    public bool CharacterInControl => _inControl;
    public Vector3 CharacterForward => transform.right;
    public Vector3 CharacterUp => transform.up;
    public Vector3 CharacterPosition => transform.position;
    public Vector3 AimingDirection => _projectileOutReference.transform.right;
    public Vector3 ProjectileOutPosition => _projectileOutReference.transform.position;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _rBody = GetComponent<Rigidbody>();
        _controlsScript = GetComponent<BaseCharacterControl>();
        _baseCollider = GetComponent<CapsuleCollider>();

        _initialLife = _life;
    }

    public virtual void Update()
    {
        UpdateFall();
    }

    public virtual void FixedUpdate()
    {
        LateUpdateFall();
    }

    public void UpdateName(string newName)
    {
        _nameTextRef.text = newName;
    }

    virtual public void AnyDamage(float amount)
    {
        _life -= amount;
        OnDamage();
    }

    virtual public void AnyDamage(int amount)
    {
        _life -= amount;
        OnDamage();
    }

    virtual public void OnDamage()
    {
        _lifeBar.fillAmount = (_life / _initialLife) * 1;
        if (CharacterInControl) GameTurnEvents.OnTurnEnd?.Invoke(null);

        if (IsDead) OnDeath();
    }

    virtual public void OnDeath()
    {
        if (_alreadyDead) return;

        _uiObjects.SetActive(false);
        _audio.PlayOneShot(_deathSound);
        GameManagerEvents.OnCharacterDeath?.Invoke(this);
        _alreadyDead = true;
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
            Vector3 fixedProjectilePos = new Vector3(ProjectileOutPosition.x, ProjectileOutPosition.y, CharacterPosition.z);

            // WIP - needs to modify to make self-damage possible
            var spawnedProjectile = Instantiate(_spawnedProjectile, fixedProjectilePos, new Quaternion());
            SphereCollider collider = spawnedProjectile.GetComponent<SphereCollider>();
            Physics.IgnoreCollision(_baseCollider, collider, true);

            IProjectile proData = spawnedProjectile.GetComponent<IProjectile>();
            proData?.UpdateDirection(_projectileOutReference.transform.right);
            proData?.UpdateSpeedMultiplier(force);

            _audio.PlayOneShot(_projectileSound);
            GameTurnEvents.OnTurnEnd?.Invoke(proData);
        }

        else // If no projectile is spawned then just end turn.
            GameTurnEvents.OnTurnEnd?.Invoke(null);

        InGameUIEvents.OnChargingWeaponBar?.Invoke(false);
    }

    virtual public void Jump()
    {
        if (!CanJump) return;

        _audio.PlayOneShot(_jumpSound);
        _rBody.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
        _fallDistance = 0;
    }

    virtual public void Jump(float jumpForce)
    {
        if (!_isInAir) return;

        _audio.PlayOneShot(_jumpSound);
        _rBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        _fallDistance = 0;
    }

    virtual public void UpdateFall()
    {
        // Main CanJump conditions
        float yVelocity = _rBody.velocity.y;
        _isInAir = yVelocity > _fallMinVelocityTolerance || yVelocity < -_fallMinVelocityTolerance;
        _isFalling = yVelocity < -_fallMinVelocityTolerance;

        // If falling, save Y position
        if (_isFalling && _fallStartingY == 0)  _fallStartingY = CharacterPosition.y;

        // When falling ends, get distance with ending Y point
        if (!_isFalling && _fallStartingY != 0)
        {
            _fallDistance = _fallStartingY - CharacterPosition.y;
            _fallStartingY = 0;
        }

        // Check if fall damage applies
        if (!_isFalling && _fallDistance > 0)
        {
            if (_fallDistance > _fallDamageMinDistance)
            {
                float extraDistance = _fallDistance - _fallDamageMinDistance;
                float damage = _fallDamageMultiplier * extraDistance;

                _audio.PlayOneShot(_fallDamageSound);
                AnyDamage(damage);
                _lastFallDistanceWithDamage = _fallDistance;
            }

            //  Reset
            _fallDistance = 0;
        }
    }

    virtual public void LateUpdateFall()
    {
        if (!_isInAir || !_isFalling) return;

        Vector3 velocity = _rBody.velocity;

        // Add "gravity" when the character is not on ground.
        if (_isInAir) velocity -= new Vector3(0, _fallSpeedModifier);

        // If falling, add more "gravity" and start fall damage checks
        if (_isFalling) velocity -= new Vector3(0, _fallSpeedModifier);

        // Don't exceed this fall speed
        if (velocity.y < -_fallMaxSpeed) velocity.y = -_fallMaxSpeed;

        // Set final velocity
        _rBody.velocity = velocity;
    }
}
