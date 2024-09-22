using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject allButtons;
    [SerializeField] private GameObject panelSettings;

    private AudioSource actionSound;

    private bool inOptionsMode = false;


    private void Start()
    {
        actionSound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        panelSettings.SetActive(inOptionsMode);
        allButtons.SetActive(!inOptionsMode);
    }


    public void PlayButton()
    {
        StartCoroutine(PlayClickSoundAndChangeScene("TurnSystemTest"));
    }

    public void SettingsButton()
    {
        inOptionsMode = true;
        actionSound.Play();
    }

    public void BackButton()
    {
        inOptionsMode = false;
        actionSound.Play();
    }

    public void CreditsButton()
    {
        StartCoroutine(PlayClickSoundAndChangeScene("Creditos"));
    }

    public void ExitButton()
    {
        StartCoroutine(PlayClickSoundAndQuit());
    }


    private IEnumerator PlayClickSoundAndChangeScene(string sceneToLoad)
    {
        actionSound.Play();

        yield return new WaitForSeconds(actionSound.clip.length);

        ChangeScene(sceneToLoad);
    }

    private IEnumerator PlayClickSoundAndQuit()
    {
        actionSound.Play();

        yield return new WaitForSeconds(actionSound.clip.length);

        Application.Quit();
    }

    private void ChangeScene(string sceneToLoad)
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
