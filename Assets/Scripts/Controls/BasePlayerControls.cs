using UnityEngine;

public class BasePlayerControls : BaseCharacterControl
{
    private bool _isChargingWeapon = false;
    public Camera _camera;
    public Vector3 InputDir => new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    public bool InputChargeWeapon => Input.GetButton("Fire");
    public bool InputJump => Input.GetButton("Jump");
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
            Character.ChargeWeapon();
        }

        if (_isChargingWeapon && !InputChargeWeapon) Character.ChargeWeaponStop();
        if (InputJump) Character.Jump();
    }

    private void FixedUpdate()
    {
        if (!Character.CharacterInControl || InputFreeLook) return;

        if (!_isChargingWeapon) Character.Move(new Vector3(InputDir.x, 0));
        if (_isChargingWeapon) Character.Aim(_camera);
    }
}
