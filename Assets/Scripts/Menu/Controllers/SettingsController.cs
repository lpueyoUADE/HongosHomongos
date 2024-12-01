using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MenuesControllerBase
{
    [Header("Other menues")]
    public GameObject mainMenuObject;


    [Header("This menu buttons")]
    public Button back;

    public override void ListenToEvents()
    {
        back.onClick.AddListener(Back);
    }

    public override void StopListenToEvents()
    {
        back.onClick.RemoveListener(Back);
    }

    private void Back()
    {
        gameObject.SetActive(false);
        MainMenuEvents.OnPlayButtonSound?.Invoke();
        mainMenuObject.SetActive(true);
    }
}
