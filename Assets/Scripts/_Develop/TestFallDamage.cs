using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestFallDamage : MonoBehaviour
{
    public GameObject Camera;
    public GameObject character;
    public GameObject platform;
    public TextMeshProUGUI text;
    public int value = 5;

    public void PlatformDisable()
    {
        platform.SetActive(true);
        platform.transform.position = new Vector3(0, value, 0);
        character.transform.position = new Vector3(0, value + 1.5f, 0);
        platform.SetActive(false);
    }

    public void RemoveAddDistance(bool add)
    {
        if (add)
        {
            if (value < 100) value += 1;
        }

        else
        {
            if (value > 1) value -= 1;
        }

        platform.SetActive(true);
        platform.transform.position = new Vector3(0, value, 0);
        character.transform.position = new Vector3(0, value + 1.5f, 0);

        text.text = value.ToString();
    }

    private void FixedUpdate()
    {
        Camera.transform.position = character.transform.position + new Vector3(0, 3, -30);
    }
}
