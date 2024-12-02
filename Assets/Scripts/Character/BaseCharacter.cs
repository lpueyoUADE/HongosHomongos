using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseCharacter : MonoBehaviour, ICharacter, IDamageable, IAbilities
{
    [Header("Character data")]
    [SerializeField] private CharacterData _characterData;
    [SerializeField, Range(0.01f, 0.25f)] private float _maxFloorDistance = 0.18f;
    [SerializeField] private GameObject _feetsReference;
    [SerializeField] private GameObject _bodyMesh;
    [SerializeField] private Animator _meshAnimation;

    [Header("Projectile settings")]
    [SerializeField] private GameObject _aimingArrowObjectAnimation;
    [SerializeField] private GameObject _weaponReference;
    [SerializeField] private GameObject _projectileOutReference;

    [Header("UI Stuff")]
    [SerializeField] private GameObject _uiObjects;
    [SerializeField] private TextMeshProUGUI _nameTextRef;
    [SerializeField] private Image _lifeBar;

    [Header("Others")] 
    [SerializeField, Range(0, 1)] private float _minVelocityToRotate = .15f;
    [SerializeField, Range(.25f, 1)] private float _abilityChangeCooldown = .35f;
    public bool _useRBodyForAnim = true;

    [Header("Status")]
    public bool _isInAir;
    public bool _isFalling;
    public float _currentAbilityChangeCooldown = 0;
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
    private string _name;
    private int _selectedAbility = 0;
    private int _currentDamageTimerTimesLeft = 0;
    private float _currentDamageTimerDamageAmount = 0;
    private int _shieldTurnTimesLeft = 0;
    private bool _isWeakened = false;
    private int _weakenedTurnTimesLeft = 0;
    private float _extraDmgWeakened = 0;
    private AbilityShield_InCharacterRotation _shieldScriptRef;
    private GameObject _weakenedParticles;

    virtual public bool IsDead => _currentLife <= 0;
    public bool CanJump => !_isInAir && !_isFalling && !_recentJump;
    public bool CharacterInControl => _inControl;
    public Vector3 CharacterForward => _bodyMesh.transform.right;
    public Vector3 CharacterUp => transform.up;
    public Vector3 CharacterPosition => transform.position;
    public CharacterData CharacterData => _characterData;
    public Animator CharacterAnimation => _meshAnimation;
    public Rigidbody CharacterRigidbody => _rBody;

    public GameObject WeaponReference => _weaponReference;
    public Vector3 AimingDirection => _projectileOutReference.transform.right;
    public Vector3 ProjectileOutPosition => _projectileOutReference.transform.position;
    public int SelectedAbility => _selectedAbility;
    public bool CanChangeAbility => _currentAbilityChangeCooldown <= 0;
    public bool IsShielding => _shieldScriptRef != null;
    public bool IsWeakened => _isWeakened;
    public string CharacterName => _name;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        _rBody = GetComponent<Rigidbody>();
        _controlsScript = GetComponent<BaseCharacterControl>();
        _baseCollider = GetComponent<CapsuleCollider>();

        _currentLife = CharacterData.Life;
        _initialLife = _currentLife;

        GameTurnEvents.OnTurnStart += OnAbilityTurnCheck;
    }

    private void Update()
    {
        UpdateFall();
        if (!IsDead) _meshAnimation.SetBool("IsFalling", _isInAir);
    }

    protected virtual void FixedUpdate()
    {
        if (!CanChangeAbility) _currentAbilityChangeCooldown -= Time.fixedDeltaTime;
        LateUpdateFall();
        LateUpdateRotation();

        if (!IsDead && _useRBodyForAnim)
        {
            if (_rBody.velocity.x > .65f || _rBody.velocity.x < -.65f) _meshAnimation.SetBool("IsMoving", true);
            else _meshAnimation.SetBool("IsMoving", false);
        }
    }

    private void OnDestroy() 
    {
        GameTurnEvents.OnTurnStart -= OnAbilityTurnCheck;
    }

    public void UpdateName(string newName)
    {
        _name = newName;
        _nameTextRef.text = _name;
    }

    virtual public void AnyDamage(float amount)
    {
        if (IsShielding) amount -= amount %= _shieldScriptRef.damageReduction;
        if (_isWeakened) amount += amount * _extraDmgWeakened;

        _currentLife -= amount;
        OnDamage();
    }

    virtual public void AnyDamage(int amount)
    {
        float newAmount = amount;
        if (IsShielding) newAmount -= newAmount %= _shieldScriptRef.damageReduction;
        if (_isWeakened) newAmount += newAmount * _extraDmgWeakened;
        _currentLife -= newAmount;
        OnDamage();
    }

    virtual public void OnDamage()
    {
        _lifeBar.fillAmount = _currentLife / _initialLife * 1;

        if (CharacterInControl) GameTurnEvents.OnTurnEnd?.Invoke(null);
        if (IsShielding) _shieldScriptRef.PlayAnim(ShieldAnim.Hit);        
        if (IsDead) OnDeath();
        else _audio.PlayOneShot(CharacterData.AnyDamageSound);
        if (CharacterData.OnDamageBloodParticles) Instantiate(CharacterData.OnDamageBloodParticles, transform);
    }

    virtual public void OnTimedDamage(float amount)
    {
        _currentLife -= amount;
        _lifeBar.fillAmount = _currentLife / _initialLife * 1;
        if (IsDead) OnDeath();
    }

    virtual public void OnDeath()
    {
        if (_alreadyDead) return;
        GameTurnEvents.OnTurnStart -= OnAbilityTurnCheck;
        _alreadyDead = true;

        _uiObjects.SetActive(false);
        _audio.PlayOneShot(CharacterData.DeathSound);
        GameManagerEvents.OnCharacterDeath?.Invoke(this);
        InGameUIEvents.OnCharacterPortraitUpdate?.Invoke(this, PortraitStatus.Dead);

        if (_shieldScriptRef) Destroy(_shieldScriptRef.gameObject);
        if (_weakenedParticles) Destroy(_weakenedParticles);
        _meshAnimation.SetBool("IsDead", true);
    }

    virtual public void InControl(bool isInControl = false)
    {
        _inControl = isInControl;
        _controlsScript.enabled = isInControl;

        if (!isInControl)
        {
            InGameUIEvents.OnChargingWeaponBar?.Invoke(false);
            InGameUIEvents.OnUpdateAbilityPortrait?.Invoke(4, true); // Clear ability portraits - will also reset ability portraits animations
            _currentAbilityChangeCooldown = 0;
            _aimingArrowObjectAnimation.SetActive(false);
        }
        else 
        {
            InGameUIEvents.OnAbilityPortrait?.Invoke(CharacterData.AbilitiesList); // Fill ability portraits
            InGameUIEvents.OnAbilityPortraitSelected?.Invoke(_selectedAbility); // Return to previosly selected ability
        }
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

        _meshAnimation.SetBool("IsAiming", true);
    }

    virtual public void ChangeAbility(int selection)
    {
        if (!CanChangeAbility || selection == _selectedAbility || _characterData.AbilitiesList.Count <= selection) return;

        _currentAbilityChangeCooldown = _abilityChangeCooldown;
        _selectedAbility = selection;
        InGameUIEvents.OnAbilityPortraitSelected?.Invoke(selection);
        InGameUIEvents.OnPlayUISound?.Invoke(GameManagerEvents.ModeSettings.AbilityChangeSound);
    }

    virtual public void ChargeAbility()
    {
        _aimingArrowObjectAnimation.SetActive(true);
        InGameUIEvents.OnChargingWeaponBar?.Invoke(true);
    }

    virtual public void ChargeAbilityStop()
    {
        _meshAnimation.SetBool("IsAiming", false);

        float force = InGameUIEvents.GetChargerBarIntensity() * CharacterData.AbilitiesList[_selectedAbility].AbilityProjectileBaseSpeed;

        // If we are in control then OnTurnEnd True will make camera follow the spawned projectile.
        if (CharacterInControl)
        {
            Vector3 fixedProjectilePos = new(ProjectileOutPosition.x, ProjectileOutPosition.y, CharacterPosition.z);
            var spawnedProjectile = Instantiate(CharacterData.AbilitiesList[_selectedAbility].AbilitProjectilePrefab, fixedProjectilePos, new Quaternion());

            // Get projectile data and set stuff
            IProjectile proData = spawnedProjectile.GetComponent<IProjectile>();
            proData.UpdateData(CharacterData.AbilitiesList[_selectedAbility], _projectileOutReference.transform.right, force, gameObject, _baseCollider);

            _audio.PlayOneShot(CharacterData.AbilitiesList[_selectedAbility].AbilitFireSound);
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
            
            // Play step sound if not damaged
            else if (_fallDistance > 0.25f) _audio.PlayOneShot(CharacterData.GroundSound);

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

    virtual public void OnAbilityHeal(float amount)
    {
        if (IsDead) return;

        _currentLife += amount;
        if (_currentLife > _characterData.Life) _currentLife = _characterData.Life;
        _lifeBar.fillAmount = _currentLife / _initialLife * 1;
    }

    virtual public void OnAbilityTimedDamage(float amount, int duration)
    {
        _currentDamageTimerDamageAmount = amount;
        _currentDamageTimerTimesLeft = duration;
        StartCoroutine(TimedDamage());
    }

    virtual public void OnAbilityShield(int duration, GameObject shieldPrefab, int damageReductionPercet)
    {
        _shieldTurnTimesLeft += duration;

        if (!IsShielding)
        {
            GameObject shieldObj = Instantiate(shieldPrefab, transform);
            _shieldScriptRef = shieldObj.GetComponent<AbilityShield_InCharacterRotation>();
        }
        else _shieldScriptRef.PlayAnim(ShieldAnim.Hit);

        _shieldScriptRef.damageReduction = damageReductionPercet;
    }

    virtual public void OnAbilityTurnCheck()
    {
        if (IsShielding)
        {
            if (_shieldTurnTimesLeft <= 0 )
            {
                _shieldScriptRef.PlayAnim(ShieldAnim.End);
                _shieldScriptRef = null;
                return;
            }

            _shieldTurnTimesLeft--;
            _shieldScriptRef.PlayAnim(ShieldAnim.Hit);
        }



        if (IsWeakened)
        {
            _weakenedTurnTimesLeft--;
            if (_weakenedTurnTimesLeft <= 0) 
            {
                _isWeakened = false;

                if (_weakenedParticles != null) _weakenedParticles.SetActive(false);
            }
        }
    }

    public void OnWeakened(int turnDurations, float extraDamageReceived, GameObject particles = null)
    {
        _weakenedTurnTimesLeft = turnDurations;
        _extraDmgWeakened = extraDamageReceived;
        _isWeakened = true;

        if (particles && _weakenedParticles == null) _weakenedParticles = Instantiate(particles, transform);
        if (_weakenedParticles) _weakenedParticles.SetActive(true);
    }

    public void OnStrengthened(int turnDurations, float extraDamageResistance)
    {
        throw new NotImplementedException();
    }

    IEnumerator TimedDamage()
    {
        yield return new WaitForSeconds(1);

        if (!IsDead || _currentDamageTimerTimesLeft > 0) 
        {
            _currentDamageTimerTimesLeft--;
            OnTimedDamage(_currentDamageTimerDamageAmount);
            StartCoroutine(TimedDamage());
        }

        else if (IsDead || _currentDamageTimerTimesLeft <= 0) _currentDamageTimerDamageAmount = 0;
    }
}
