using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Wireframe : MonoBehaviour
{
    [SerializeField] private GameObject[] panels;
    public string sceneName = "UpToDateTest";

    private AudioSource soundOption;


    void Start()
    {
        soundOption = GetComponent<AudioSource>();

        ShowCurrentPanel(0);
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
        }
    }

    private IEnumerator PlayClickSoundAndLoadScene(string sceneName)
    {
        soundOption.Play();
        yield return new WaitForSeconds(soundOption.clip.length);
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator PlayClickSoundAndQuitGmae()
    {
        soundOption.Play();
        yield return new WaitForSeconds(soundOption.clip.length);
        Application.Quit();
    }


    public void BackButton(int indexPanel)
    {
        soundOption.Play();
        ShowCurrentPanel(indexPanel);    
    }


    // panel Main Menu \\
    public void PlayButton()
    {
        soundOption.Play();
        ShowCurrentPanel(2);
    }

    public void SettingsButton()
    {
        soundOption.Play();
        ShowCurrentPanel(1);
    }

    public void CrditsButton()
    {
        soundOption.Play();
        StartCoroutine(PlayClickSoundAndLoadScene("Credits"));
    }

    public void QuitButton()
    {
        soundOption.Play();
        PlayClickSoundAndQuitGmae();
    }


    // panel New Game \\
    public void QuickGameButton()
    {
        soundOption.Play();
        ShowCurrentPanel(3);
    }

    public void CareerButton()
    {
        soundOption.Play();
        ShowCurrentPanel(4);
    }


    // panel Quick Game and Career \\
    public void CP1Level1()
    {
        soundOption.Play();
        ShowCurrentPanel(5);
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


    // panel Battle preparations \\

    public void StartBattle()
    {
        soundOption.Play();
        StartCoroutine(PlayClickSoundAndLoadScene(sceneName));
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

    public void Item()
    {

    }

    public void SellSelected()
    {

    }
}
