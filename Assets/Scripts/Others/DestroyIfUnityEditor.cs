using UnityEngine;

public class DestroyIfUnityEditor : MonoBehaviour
{
    private void Awake() 
    {
        
# if UNITY_EDITOR

# else
    Destroy(gameObject);
# endif

    }
}
