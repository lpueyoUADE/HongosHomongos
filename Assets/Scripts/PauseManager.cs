using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    private static PauseManager instance;
    public static PauseManager Instance {  get { return instance; } }   

    private AudioSource actionSound;

    private GameObject panel;
    private GameObject Buttons;

    private bool isGamePaused = false;

    public bool IsGamePaused { get => isGamePaused; }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        actionSound = GetComponent<AudioSource>();

        panel = transform.Find("Panel").gameObject;
        Buttons = transform.Find("Buttons").gameObject;
    }

    void Update()
    {
        panel.SetActive(isGamePaused);
        Buttons.SetActive(isGamePaused);

        PauseGame();
    }


    public void ResumeGame()
    {
        actionSound.Play();
        isGamePaused = false;
    }

    public void ReturnToMenu()
    {
        StartCoroutine(PlayClickSoundAndChangeScene("Menu"));
        isGamePaused = false;
    }


    private void PauseGame()
    {
        if (!isGamePaused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                actionSound.Play();
                isGamePaused = true;
            }
        }

        else if (isGamePaused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                actionSound.Play();
                isGamePaused = false;
            }        
        }

        Time.timeScale = isGamePaused ? 0f : 1f; 
    }

    private IEnumerator PlayClickSoundAndChangeScene(string sceneToLoad)
    {
        actionSound.Play();

        yield return new WaitForSeconds(actionSound.clip.length);

        ChangeScene(sceneToLoad);
    }

    private void ChangeScene(string sceneToLoad)
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
