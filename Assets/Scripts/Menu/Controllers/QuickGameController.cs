using UnityEngine;
using UnityEngine.UI;

public class QuickGameController : MenuesControllerBase
{
    [Header("Other menues")]
    public GameObject playObject;


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
        playObject.SetActive(true);
    }
}
