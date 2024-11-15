using UnityEngine;

[CreateAssetMenu(fileName = "NewUserConfigData", menuName = "Databases/UserConfig")]
public class UserConfigData : ScriptableObject 
{
    [Range(.1f, 1)] public float FreeLookSpeed = 0.25f;
    
}
