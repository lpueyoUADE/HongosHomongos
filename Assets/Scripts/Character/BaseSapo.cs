using UnityEngine;

public class BaseSapo : BaseCharacter, ISapo
{
    private void Start()
    {
        transform.rotation = new Quaternion(0, 90, 0, 0);
    }
}
