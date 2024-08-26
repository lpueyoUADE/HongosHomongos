using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class logicaVolumen : MonoBehaviour
{
    public Slider slider;
    public TMP_Text porcentajeVolumen;

    public float sliderValor;


    void Start()
    {
        slider.value = PlayerPrefs.GetFloat("volumenAudio", 0.5f);
        AudioListener.volume = slider.value;
        actualizarPorcentaje();
    }

    public void cambiarSlider(float valor)
    {
        sliderValor = valor;
        PlayerPrefs.SetFloat("volumenAudio", sliderValor);
        AudioListener.volume = slider.value;
        actualizarPorcentaje();
    }

    public void actualizarPorcentaje()
    {
        porcentajeVolumen.text = (slider.value * 100f).ToString("F0") + "%";
    }
}

