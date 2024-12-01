using UnityEngine;

public class BasePlayerControls : BaseCharacterControl
{
    private bool _isChargingWeapon = false;
    public Camera _camera;
    public Vector3 InputDir => new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    public bool InputChargeWeapon => Input.GetButton("Fire");
    public bool InputJump => Input.GetButton("Jump");
    public bool InputAbility1 => Input.GetButtonDown("Ability1");
    public bool InputAbility2 => Input.GetButtonDown("Ability2");
    public bool InputAbility3 => Input.GetButtonDown("Ability3");
    public bool InputFreeLook => Input.GetButton("FreeLook");

    private void Start() 
    {
        _camera = CameraEvents.Cam;
    }

    private void OnDisable()
    {
        if (_isChargingWeapon) InGameUIEvents.OnChargingWeaponBar?.Invoke(false);
        _isChargingWeapon = false;
        CameraEvents.OnEndFreeLookMode?.Invoke();
    }

    private void Update()
    {
        if (GameTurnEvents.IsGamePaused || GameTurnEvents.IsGameFinished) return;

        if (!Character.CharacterInControl) return;

        if (InputFreeLook && !_isChargingWeapon)
        {
            CameraEvents.OnStartFreeLookMode?.Invoke(InputDir);
            return;
        }
        else if (!InputFreeLook) CameraEvents.OnEndFreeLookMode?.Invoke();

        if (InputChargeWeapon)
        {
            _isChargingWeapon = true;
            Character.ChargeAbility();
        }

        if (_isChargingWeapon && !InputChargeWeapon) Character.ChargeAbilityStop();
        if (InputJump) Character.Jump();

        if (InputFreeLook || _isChargingWeapon) return;

        if (InputAbility1 || InputAbility2 || InputAbility3)
        {
            if (InputAbility1)
            {
                Character.ChangeAbility(0);
                return;
            }
            if (InputAbility2)
            {
                Character.ChangeAbility(1);
                return;
            }
            if (InputAbility3) Character.ChangeAbility(2);
        }
    }

    private void FixedUpdate()
    {
        if (GameTurnEvents.IsGamePaused || GameTurnEvents.IsGameFinished) return;
        if (!Character.CharacterInControl || InputFreeLook) return;

        if (!_isChargingWeapon) Character.Move(new Vector3(InputDir.x, 0));
        if (_isChargingWeapon) Character.Aim(_camera);
    }
}
