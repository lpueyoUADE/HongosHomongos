using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    public void GoToMainMenu()
    { 
        SceneManager.LoadScene("Menu");
    }

}
