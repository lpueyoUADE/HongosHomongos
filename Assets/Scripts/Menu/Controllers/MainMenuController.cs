using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MenuesControllerBase
{
    public SceneData demoScene;

    [Header("Other menues")]
    public GameObject playObject;
    public GameObject settingsObject;
    public GameObject creditsObject;
    public GameObject demoSceneBattlePreparations;


    [Header("This menu buttons")]
    public Button mainMenuPlay;
    public Button mainMenuSettings;
    public Button mainMenuCredits;
    public Button mainMenuExit;
    public Button mainMenuDemoScene;
    
    public override void ListenToEvents()
    {
        mainMenuPlay.onClick.AddListener(OnMainMenuPlayButton);
        mainMenuSettings.onClick.AddListener(OnMainMenuSettingsButton);
        mainMenuCredits.onClick.AddListener(OnMainMenuCreditsButton);
        mainMenuExit.onClick.AddListener(OnMainMenuExitButton);
        
        mainMenuDemoScene.onClick.AddListener(OnMainMenuDemoSceneButton);

    }

    public override void StopListenToEvents()
    {
        mainMenuPlay.onClick.RemoveListener(OnMainMenuPlayButton);
        mainMenuSettings.onClick.RemoveListener(OnMainMenuSettingsButton);
        mainMenuCredits.onClick.RemoveListener(OnMainMenuCreditsButton);
        mainMenuExit.onClick.RemoveListener(OnMainMenuExitButton);

        mainMenuDemoScene.onClick.RemoveListener(OnMainMenuDemoSceneButton);
    }

    private void OnMainMenuPlayButton()
    {
        gameObject.SetActive(false);
        MainMenuEvents.OnPlayButtonSound?.Invoke();
        playObject.SetActive(true);
    }
    private void OnMainMenuSettingsButton()
    {
        gameObject.SetActive(false);
        MainMenuEvents.OnPlayButtonSound?.Invoke();
        settingsObject.SetActive(true);
    }
    private void OnMainMenuCreditsButton()
    {
        gameObject.SetActive(false);
        MainMenuEvents.OnPlayButtonSound?.Invoke();
        creditsObject.SetActive(true);
    }
    private void OnMainMenuExitButton()
    {
        Application.Quit();
        
# if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
# endif
    }

    private void OnMainMenuDemoSceneButton()
    {
        demoSceneBattlePreparations.SetActive(true);
        BattlePreparationsController.OnUpdateScenarioSelected?.Invoke(demoScene);
        gameObject.SetActive(false);
        MainMenuEvents.OnPlayButtonSound?.Invoke();
        
    }
}
