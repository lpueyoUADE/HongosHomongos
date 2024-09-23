using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Wireframe : MonoBehaviour
{
    [SerializeField] private GameObject[] panels;

    private int currentPanelIndex = 0;


    void Start()
    {
        ShowCurrentPanel(currentPanelIndex);
    }

    private void ShowCurrentPanel(int panelIndex)
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }

        if (panelIndex >= 0 && panelIndex < panels.Length)
        {
            panels[panelIndex].SetActive(true);
            currentPanelIndex = panelIndex;
        }
    }


    public void BackButton(int indexPanel)
    {
        ShowCurrentPanel(indexPanel);    
    }


    // panel Main Menu
    public void PlayButton()
    {
        ShowCurrentPanel(1);
    }

    public void SettingsButton()
    {

    }

    public void CrditsButton()
    {
        SceneManager.LoadScene("Crdits");
    }

    public void QuitButton()
    {
        Application.Quit();
    }


    // panel New Game
    public void QuickGameButton()
    {
        ShowCurrentPanel(2);
    }

    public void CareerButton()
    {
        ShowCurrentPanel(3);
    }


    // panel Quick Game
    public void CP1Level1()
    {

    }

    public void CP1Level2()
    {

    }

    public void CP1Level3()
    {

    }

    public void CP1LevelBoss()
    {

    }

    public void CP2Level1()
    {

    }

    public void CP2Level2()
    {

    }

    public void CP2Level3()
    {

    }

    public void CP2LevelBoss()
    {

    }

    public void CP3Level1()
    {

    }

    public void CP3Level2()
    {

    }

    public void CP3Level3()
    {

    }

    public void CP3LevelBoss()
    {

    }
    public void Career()
    {
        ShowCurrentPanel(3);
    }


    // panel Battle preparations for career mode and Quick game

    public void StartBattle()
    {
        SceneManager.LoadScene("TurnSystemTest");
    }

    public void StartQuickGame()
    {
        SceneManager.LoadScene("Aca iria el nombre de la escena");
    }

    public void Item()
    {

    }

    public void BuyWarrior()
    {

    }

    public void BuyArcher()
    {

    }

    public void BuyMage()
    {

    }

    public void SellSelected()
    {

    }
}
