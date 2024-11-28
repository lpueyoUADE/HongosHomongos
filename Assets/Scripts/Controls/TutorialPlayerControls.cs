using UnityEngine;

public class TutorialPlayerControls : BaseCharacterControl
{
    public bool _isChargingWeapon = false;
    public Camera _camera;
    public Vector3 InputDir => new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    public bool InputChargeWeapon => Input.GetButton("Fire") && tutCanUseChargeBar;
    public bool InputJump => Input.GetButton("Jump") && tutCanJump;
    public bool InputAbility1 => Input.GetButtonDown("Ability1") && tutCanChangeAbility;
    public bool InputAbility2 => Input.GetButtonDown("Ability2") && tutCanChangeAbility;
    public bool InputAbility3 => Input.GetButtonDown("Ability3") && tutCanChangeAbility;
    public bool InputFreeLook => Input.GetButton("FreeLook") & tutCanUseFreeLook;

    [Header("Tutorial stuff")]
    public bool tutCanMove = false;
    public bool tutCanJump = false;
    public bool tutCanUseChargeBar = false;
    public bool tutCanChangeAbility = false;
    public bool tutCanUseFreeLook = false;


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

    protected virtual void Update()
    {       
        if (!Character.CharacterInControl) return;

        if (InputFreeLook && !_isChargingWeapon)
        {
            TutorialControls.OnKeyPressed?.Invoke("freelook");
            CameraEvents.OnStartFreeLookMode?.Invoke(InputDir);
            return;
        }
        else if (!InputFreeLook) CameraEvents.OnEndFreeLookMode?.Invoke();

        if (InputChargeWeapon)
        {
            TutorialControls.OnKeyPressed?.Invoke("lmouse");
            _isChargingWeapon = true;
            Character.ChargeAbility();
        }

        if (_isChargingWeapon && !InputChargeWeapon) Character.ChargeAbilityStop();
        if (InputJump) 
        {
            Character.Jump();
            TutorialControls.OnKeyPressed?.Invoke("jump");
        }

        if (InputFreeLook || _isChargingWeapon) return;

        if (InputAbility1 || InputAbility2 || InputAbility3)
        {
            if (InputAbility1)
            {
                TutorialControls.OnKeyPressed?.Invoke("ab1");
                Character.ChangeAbility(0);
                return;
            }
            if (InputAbility2)
            {
                TutorialControls.OnKeyPressed?.Invoke("ab2");
                Character.ChangeAbility(1);
                return;
            }
            if (InputAbility3)
            {
                TutorialControls.OnKeyPressed?.Invoke("ab3");
                Character.ChangeAbility(2);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!tutCanMove) return;
        if (InputDir.x < 0) TutorialControls.OnKeyPressed?.Invoke("a");
        if (InputDir.x > 0) TutorialControls.OnKeyPressed?.Invoke("d");

        
        if (!Character.CharacterInControl || InputFreeLook) return;

        if (!_isChargingWeapon) Character.Move(new Vector3(InputDir.x, 0));
        if (_isChargingWeapon) Character.Aim(_camera);
    }
}
