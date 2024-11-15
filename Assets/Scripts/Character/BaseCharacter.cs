using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseCharacter : MonoBehaviour, ICharacter, IDamageable
{
    [Header("Character data")]
    [SerializeField] private CharacterData _characterData;
    [SerializeField, Range(0.01f, 0.25f)] private float _maxFloorDistance = 0.18f;
    [SerializeField] private GameObject _feetsReference;
    [SerializeField] private GameObject _bodyMesh;

    [Header("Projectile settings")]
    [SerializeField] private GameObject _weaponReference;
    [SerializeField] private GameObject _projectileOutReference;

    [Header("UI Stuff")]
    [SerializeField] private GameObject _uiObjects;
    [SerializeField] private TextMeshProUGUI _nameTextRef;
    [SerializeField] private Image _lifeBar;

    [Header("Offsets")] 
    [SerializeField, Range(0, 1)] private float _minVelocityToRotate = .15f;

    [Header("Status")]
    public bool _isInAir;
    public bool _isFalling;
    [Tooltip("Not used - only for show.")] public float _lastFallDistanceWithDamage;

    // Values
    private float _currentLife = 1;
    private float _initialLife;
    private bool _alreadyDead;
    private AudioSource _audio;
    private BaseCharacterControl _controlsScript;
    private CapsuleCollider _baseCollider;
    private Rigidbody _rBody;
    private bool _inControl = false;
    private float _fallStartingY;
    private float _fallDistance;
    private bool _recentJump = false;
    private bool _isAiming;
    private string _name;

    virtual public bool IsDead => _currentLife <= 0;
    public bool CanJump => !_isInAir && !_isFalling && !_recentJump;
    public bool CharacterInControl => _inControl;
    public Vector3 CharacterForward => _bodyMesh.transform.right;
    public Vector3 CharacterUp => transform.up;
    public Vector3 CharacterPosition => transform.position;
    public CharacterData CharacterData => _characterData;

    public GameObject WeaponReference => _weaponReference;
    public Vector3 AimingDirection => _projectileOutReference.transform.right;
    public Vector3 ProjectileOutPosition => _projectileOutReference.transform.position;

    public string CharacterName => _name;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _rBody = GetComponent<Rigidbody>();
        _controlsScript = GetComponent<BaseCharacterControl>();
        _baseCollider = GetComponent<CapsuleCollider>();

        _currentLife = CharacterData.Life;
        _initialLife = _currentLife;
    }

    public virtual void Update()
    {
        UpdateFall();
    }

    public virtual void FixedUpdate()
    {
        LateUpdateFall();
        LateUpdateRotation();
    }

    public void UpdateName(string newName)
    {
        _name = newName;
        _nameTextRef.text = _name;
    }

    virtual public void AnyDamage(float amount)
    {
        _currentLife -= amount;
        OnDamage();
    }

    virtual public void AnyDamage(int amount)
    {
        _currentLife -= amount;
        OnDamage();
    }

    virtual public void OnDamage()
    {
        _lifeBar.fillAmount = _currentLife / _initialLife * 1;
        if (CharacterInControl) GameTurnEvents.OnTurnEnd?.Invoke(null);

        if (IsDead) OnDeath();
    }

    virtual public void OnDeath()
    {
        if (_alreadyDead) return;

        _uiObjects.SetActive(false);
        _audio.PlayOneShot(CharacterData.DeathSound);
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
        direction.y = 0;
        _rBody.AddForce(direction * speed, mode);
    }

    virtual public void Move(Vector3 direction, ForceMode mode = ForceMode.Impulse)
    {
        direction.y = 0;
        _rBody.AddForce(direction * CharacterData.Speed / 2, mode);
    }

    virtual public void Aim(Vector3 direction)
    {
        WeaponReference.transform.Rotate(Vector3.forward, direction.y);
    }

    virtual public void Aim(Vector3 direction, float speed)
    {
        WeaponReference.transform.rotation = Quaternion.Euler(direction.x, direction.y, direction.y * speed);
    }

    virtual public void Aim(Camera cam)
    {
        // Get mouse position
        Vector3 position = WeaponReference.transform.position;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = position.z - cam.transform.position.z;

        // Set angle
        Vector3 thisPosToWorld = Camera.main.WorldToScreenPoint(position);
        mousePosition.x -= thisPosToWorld.x;
        mousePosition.y -= thisPosToWorld.y;
        float angle = Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg;

        // Set rotation
        WeaponReference.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
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
            var spawnedProjectile = Instantiate(CharacterData.Projectile, fixedProjectilePos, new Quaternion());
            SphereCollider collider = spawnedProjectile.GetComponent<SphereCollider>();
            Physics.IgnoreCollision(_baseCollider, collider, true);

            IProjectile proData = spawnedProjectile.GetComponent<IProjectile>();
            proData?.UpdateDirection(_projectileOutReference.transform.right);
            proData?.UpdateSpeedMultiplier(force);

            _audio.PlayOneShot(CharacterData.ProjectileSound);
            GameTurnEvents.OnTurnEnd?.Invoke(proData);
        }

        else // If no projectile is spawned then just end turn.
            GameTurnEvents.OnTurnEnd?.Invoke(null);

        InGameUIEvents.OnChargingWeaponBar?.Invoke(false);
    }

    virtual public void Jump()
    {
        if (!CanJump) return;

        _recentJump = true;
        _audio.PlayOneShot(CharacterData.JumpSound);
        _rBody.AddForce(transform.up * CharacterData.JumpForce, ForceMode.Impulse);
        _fallDistance = 0;
    }

    virtual public void Jump(float jumpForce)
    {
        if (!_isInAir) return;

        _audio.PlayOneShot(CharacterData.JumpSound);
        _rBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        _fallDistance = 0;
    }

    virtual public void UpdateFall()
    {
        // Main CanJump conditions
        float yVelocity = _rBody.velocity.y;
        Vector3 feetsPos = _feetsReference.transform.position;
        bool floorDetected =    Physics.Raycast(feetsPos, Vector3.down, _maxFloorDistance, CharacterData.FloorMask) ||
                                Physics.Raycast(feetsPos + Vector3.left *.25f, Vector3.down, _maxFloorDistance, CharacterData.FloorMask) ||
                                Physics.Raycast(feetsPos + Vector3.right *.25f, Vector3.down, _maxFloorDistance, CharacterData.FloorMask);

        _isInAir = yVelocity > CharacterData.FallMinVelocityTolerance || yVelocity < -CharacterData.FallMinVelocityTolerance || !floorDetected;
        _isFalling = yVelocity < -CharacterData.FallMinVelocityTolerance;

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
            if (_fallDistance > CharacterData.FallDamageMinDistance)
            {
                float extraDistance = _fallDistance - CharacterData.FallDamageMinDistance;
                float damage = CharacterData.FallDamageMultiplier * extraDistance;

                _audio.PlayOneShot(CharacterData.FallDamageSound);
                AnyDamage(damage);
                _lastFallDistanceWithDamage = _fallDistance;
            }

            //  Reset
            _fallDistance = 0;
            if (_recentJump) _recentJump = false;
        }
    }

    virtual public void LateUpdateFall()
    {
        if (!_isInAir || !_isFalling) return;

        Vector3 velocity = _rBody.velocity;

        // Add "gravity" when the character is not on ground.
        if (_isInAir) velocity -= new Vector3(0, CharacterData.FallSpeedModifier);

        // If falling, add more "gravity" and start fall damage checks
        if (_isFalling) velocity -= new Vector3(0, CharacterData.FallSpeedModifier);

        // Don't exceed this fall speed
        if (velocity.y < -CharacterData.FallMaxSpeed) velocity.y = -CharacterData.FallMaxSpeed;

        // Set final velocity
        _rBody.velocity = velocity;
    }

    virtual public void LateUpdateRotation()
    {
        if (!_inControl || _rBody.velocity.x == 0) return;

        float xVel = _rBody.velocity.x;
        if (xVel < _minVelocityToRotate && xVel < 0) _bodyMesh.transform.rotation = new Quaternion(0, 180, 0, 0);
        if (xVel > -_minVelocityToRotate && xVel > 0)  _bodyMesh.transform.rotation = new Quaternion(0, 0, 0, 0);

    }
}
