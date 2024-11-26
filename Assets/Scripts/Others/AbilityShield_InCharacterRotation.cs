using UnityEngine;


public enum ShieldAnim{
    Hit, End
}
public class AbilityShield_InCharacterRotation : MonoBehaviour
{
    [Range(.1f, 3)] public float speed = 1.5f;
    public int damageReduction = 0;
    public Animator _anim;

    private void FixedUpdate() 
    {
        transform.Rotate( new Vector3(0, speed, 0));
    }

    public void PlayAnim(ShieldAnim anim)
    {
        switch (anim)
        {
            case ShieldAnim.Hit:
                _anim.SetTrigger("InHit");
                break;
            case ShieldAnim.End:
                _anim.SetTrigger("InEnd");
                break;
        }
    }

    public void SelfDestruct()
    {
        Destroy(this.gameObject);
    }
}
 