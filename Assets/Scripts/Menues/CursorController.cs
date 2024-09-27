using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorController : MonoBehaviour
{
    private static CursorController instance;
    public static CursorController Instance { get { return instance; } }

    private Scene currentScene;


    void Awake()
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

    void Update()
    {
        currentScene = SceneManager.GetActiveScene();

        switch (currentScene.name)
        {
            case "Menu": case "UI":

                Cursor.visible = true;

            break;

            case "TurnSystemTest":

                if (!PauseManager.Instance.IsGamePaused)
                {
                    Cursor.visible = false;
                }

                else
                {
                    Cursor.visible = true;
                }

            break;
        }
    }
}
