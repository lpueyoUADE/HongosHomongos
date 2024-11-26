using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum AbilityStatus
{
    Idle, Selected, Disabled
}

public class InGameUIAbilityPortrait : MonoBehaviour
{
    private Image _sprite;
    private Animator _anim;
    public TextMeshProUGUI _nameText;

    [SerializeField] private TextMeshProUGUI _hotkeyIndicator;

    private void Awake() 
    {
        _sprite = GetComponent<Image>();
        _anim = GetComponent<Animator>();
    }

    public void UpdatePortrait(Sprite newImage, int newHotkey, string newName)
    {
        _sprite.sprite = newImage;
        _hotkeyIndicator.text = $"{newHotkey}";
        _nameText.text = newName;
    }

    public void UpdatePortrait(Sprite newImage)
    {
        _sprite.sprite = newImage;
    }

    public void UpdatePortrait(int newHotkey)
    {
        _hotkeyIndicator.text = $"{newHotkey}";
    }

    public void UpdateAnim(AbilityStatus newStatus)
    {
        switch (newStatus)
        {
            case AbilityStatus.Idle:
                _anim.SetTrigger("InIdle");
            break;

            case AbilityStatus.Selected:
            _anim.SetTrigger("InSelected");
            break;

            case AbilityStatus.Disabled:
            _anim.SetTrigger("InDisabled");
            _nameText.text = "";
            break;
        }
    }
}
