using UnityEngine;

public class ProjectileArrowLikeRotation : MonoBehaviour
{
    [SerializeField] private Rigidbody _rBody;

    private void FixedUpdate() 
    {
        if (_rBody == null) return;
        transform.LookAt(transform.position - _rBody.velocity);
        transform.Rotate(0, 0 , 90);
    }
}
