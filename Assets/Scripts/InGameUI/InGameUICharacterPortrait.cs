using UnityEngine;
using UnityEngine.UI;

public class InGameUICharacterPortrait : MonoBehaviour
{
    public Image _sprite;
    public Animator _anim;

    public void UpdateCharacterPortrait(Sprite newSprite)
    {
        _sprite.sprite = newSprite;
    }

    public void UpdatePortraitAnimation(PortraitStatus newStatus)
    {
        switch (newStatus)
        {
            case PortraitStatus.Idle: _anim.SetTrigger("InIdle"); break;
            case PortraitStatus.CurrentTurn: _anim.SetTrigger("InTurn"); break;
            case PortraitStatus.LookAt: _anim.SetTrigger("InLookAt"); break;
            case PortraitStatus.Dead: _anim.SetTrigger("InDead"); break;            
        }
    }
}
