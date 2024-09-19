using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerControls : BaseCharacterControl
{
    private bool _isChargingWeapon = false;

    public Vector3 InputDir => new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    public bool InputChargeWeapon => Input.GetButton("Jump");

    private void OnDisable()
    {
        _isChargingWeapon = false;
    }

    private void Update()
    {
        if (!Character.CharacterInControl) return;

        // Running out of time while aiming
        if (!Character.CharacterInControl && _isChargingWeapon)
        {
            _isChargingWeapon = false;
            InGameUIEvents.OnChargingWeaponBar?.Invoke(false);
            return;
        }

        if (InputChargeWeapon)
        {
            _isChargingWeapon = true;
            Character.ChargeWeapon();
        }

        if (Character.CharacterInControl && _isChargingWeapon && !InputChargeWeapon)
        {
            Character.ChargeWeaponStop();
        }
    }

    private void FixedUpdate()
    {
        if (!Character.CharacterInControl) return;
        Character.Aim(InputDir * _aimSpeed);
        Character.Move(new Vector3(InputDir.x, 0));
    }
}
