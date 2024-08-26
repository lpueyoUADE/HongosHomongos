using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FPScounter : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    public Toggle toggle;

    private float pollingTime = 1f;
    private float time;
    private int frameCount;

    private void Start()
    {
        toggle.isOn = PlayerPrefs.GetInt("FPSToggle", 1) == 1;
        ToggleText(toggle.isOn);

       
        toggle.onValueChanged.AddListener(ToggleText);
    }

    private void Update()
    {
        time += Time.deltaTime;
        frameCount++;

        if (time >= pollingTime)
        {
            int frameRate = Mathf.RoundToInt(frameCount / time);

            if (fpsText.gameObject.activeSelf)
                fpsText.text = frameRate.ToString() + " FPS";

            time -= pollingTime;
            frameCount = 0;
        }
    }

    public void ToggleText(bool isOn)
    {
        fpsText.gameObject.SetActive(isOn);
        PlayerPrefs.SetInt("FPSToggle", isOn ? 1 : 0);
        PlayerPrefs.Save(); 
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("FPSToggle", toggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}
