using UnityEngine;

public interface ICharacter
{
    void Move(Vector3 direction, float speed, ForceMode mode = ForceMode.Impulse);
    void Move(Vector3 direction, ForceMode mode = ForceMode.Impulse);
    void Aim(Vector3 direction);
    void Jump();
    void Jump(float jumpForce);
    void UpdateFall();
    void LateUpdateFall();

    void InControl(bool isInControl = false);
    void UpdateName(string newName);
}
