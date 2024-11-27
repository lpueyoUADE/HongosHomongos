using UnityEngine;

public class ProjectileArrowLikeRotation : MonoBehaviour
{
    [SerializeField] private Rigidbody _rBody;

    private void FixedUpdate() 
    {
        transform.LookAt(transform.position - _rBody.velocity);
        transform.Rotate(0, 0 , 90);
    }
}
