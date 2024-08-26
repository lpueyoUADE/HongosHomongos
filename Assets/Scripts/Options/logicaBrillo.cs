using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class logicaBrillo : MonoBehaviour
{
    public Slider slider;
    public Image panelBrillo;
    public TMP_Text porcentajeBrillo;

    public float sliderValor;

    void Start()
    {
        sliderValor = PlayerPrefs.GetFloat("brillo", 0.5f);
        slider.value = sliderValor;
        cambiarBrillo(sliderValor);
    }

    public void cambiarSlider(float valor)
    {
        sliderValor = valor;
        PlayerPrefs.SetFloat("brillo", sliderValor);
        cambiarBrillo(sliderValor);
    }

    void cambiarBrillo(float valor)
    {
        if (valor >= 1f || valor >= 0.9002644) 
        {
            panelBrillo.color = new Color(panelBrillo.color.r, panelBrillo.color.g, panelBrillo.color.b, 225f / 255f);
        }
        else
        {
            panelBrillo.color = new Color(panelBrillo.color.r, panelBrillo.color.g, panelBrillo.color.b, valor);
        }
        actualizarPorcentaje();
    }

    void actualizarPorcentaje()
    {
        if (slider.direction == Slider.Direction.RightToLeft)
            porcentajeBrillo.text = Mathf.RoundToInt((1 - sliderValor) * 100) + "%";
        else
            porcentajeBrillo.text = Mathf.RoundToInt(sliderValor * 100) + "%";
    }
}
