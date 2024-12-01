using UnityEngine;
using UnityEngine.UI;

public class PlayController : MenuesControllerBase
{
    [Header("Other menues")]
    public GameObject quickGameObject;
    public GameObject careerObject;
    public GameObject mainMenuObject;


    [Header("This menu buttons")]
    public Button quickGame;
    public Button career;
    public Button back;

    public override void ListenToEvents()
    {
        quickGame.onClick.AddListener(ToQuickGame);
        career.onClick.AddListener(ToCareer);
        back.onClick.AddListener(Back);
    }

    public override void StopListenToEvents()
    {
        quickGame.onClick.RemoveListener(ToQuickGame);
        career.onClick.RemoveListener(ToCareer);
        back.onClick.RemoveListener(Back);
    }

    private void ToQuickGame()
    {
        gameObject.SetActive(false);
        MainMenuEvents.OnPlayButtonSound?.Invoke();
        quickGameObject.SetActive(true);
    }
    private void ToCareer()
    {
        gameObject.SetActive(false);
        MainMenuEvents.OnPlayButtonSound?.Invoke();
        careerObject.SetActive(true);
    }
    private void Back()
    {
        gameObject.SetActive(false);
        MainMenuEvents.OnPlayButtonSound?.Invoke();
        mainMenuObject.SetActive(true);
    }
}
