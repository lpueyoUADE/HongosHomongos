using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattlePreparationsController : MenuesControllerBase
{
    public static Action<SceneData> OnUpdateScenarioSelected;
    private string _sceneToOpen;

    [Header("Other menues")]
    public GameObject mainMenuObject;


    [Header("This menu buttons")]
    public Button back;
    public Button startBattle;

    [Header("References")]
    public TextMeshProUGUI textSceneName;

    public override void ListenToEvents()
    {
        OnUpdateScenarioSelected += UpdateStuff;
        back.onClick.AddListener(Back);
        startBattle.onClick.AddListener(OnStartBattle);
    }

    public override void StopListenToEvents()
    {
        OnUpdateScenarioSelected -= UpdateStuff;
        back.onClick.RemoveListener(Back);
        startBattle.onClick.RemoveListener(OnStartBattle);
    }

    private void OnStartBattle()
    {
        StopListenToEvents();
        StartCoroutine(GotoLevel(_sceneToOpen));
    }

    private void Back()
    {
        gameObject.SetActive(false);
        MainMenuEvents.OnPlayButtonSound?.Invoke();
        mainMenuObject.SetActive(true);
    }

    private void UpdateStuff(SceneData sceneData)
    {
        _sceneToOpen = sceneData.SceneName;
        textSceneName.text = sceneData.SceneNameToUser;
    }

    IEnumerator GotoLevel(string scene)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
