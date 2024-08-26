using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class fpsOptions : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    private const string FPS_KEY = "SelectedFPS";

    private void Start()
    {
        if (PlayerPrefs.HasKey(FPS_KEY))
        {
            int savedFPSIndex = PlayerPrefs.GetInt(FPS_KEY);
            dropdown.value = savedFPSIndex;
            OnDropdownValueChanged(savedFPSIndex);
        }
        else
        {
            dropdown.value = dropdown.options.Count - 1;
            OnDropdownValueChanged(dropdown.value);
        }

        dropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(dropdown.value); });
    }

    public void OnDropdownValueChanged(int value)
    {
        string selectedOption = dropdown.options[value].text;
        int selectedFPS;

        if (selectedOption == "ILIMITADO")
        {
            selectedFPS = int.MaxValue;
        }
        else
        {
            selectedFPS = int.Parse(selectedOption.Replace(" FPS", ""));
        }

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = selectedFPS;

        PlayerPrefs.SetInt(FPS_KEY, value);
        PlayerPrefs.Save();
    }
}
