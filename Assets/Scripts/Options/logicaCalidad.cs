using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class logicaCalidad : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public int calidad;

    public bool idiomaIngles;
    public bool idiomaEspañol = false;
    public bool idiomaRuso = false;
    public bool idiomaAleman = false;
    public bool idiomaItaliano = false;
    public bool idiomaPortugues = false;


    private void Update()
    {
        AsignarNombresDeTextos();
    }

    private void Awake()
    {
        idiomaIngles = true;
        idiomaIngles = PlayerPrefs.GetInt("ingles", 0) == 1;
        idiomaEspañol = PlayerPrefs.GetInt("español", 0) == 1;
        idiomaRuso = PlayerPrefs.GetInt("ruso", 0) == 1;
        idiomaAleman = PlayerPrefs.GetInt("aleman", 0) == 1;
        idiomaItaliano = PlayerPrefs.GetInt("italiano", 0) == 1;
        idiomaPortugues = PlayerPrefs.GetInt("portugues", 0) == 1;
    }

    void Start()
    {
        calidad = PlayerPrefs.GetInt("numeroDeCalidad", 3);
        dropdown.value = calidad;
        ajustarCalidad();
        AsignarNombresDeTextos();
    }

    public void ajustarCalidad()
    {
        QualitySettings.SetQualityLevel(dropdown.value);
        PlayerPrefs.SetInt("numeroDeCalidad", dropdown.value);
        calidad = dropdown.value;
    }

    public void AsignarNombresDeTextos()
    {
        if (dropdown.options.Count >= 3 && idiomaIngles == true)
        {
            dropdown.options[0].text = "Low";
            dropdown.options[1].text = "Very Low";
            dropdown.options[2].text = "Medium";
            dropdown.options[3].text = "High";
            dropdown.options[4].text = "Very High";
            dropdown.options[5].text = "Ultra";
        }

        if (dropdown.options.Count >= 3 && idiomaEspañol == true)
        {
            dropdown.options[0].text = "Bajo";
            dropdown.options[1].text = "Muy Bajo";
            dropdown.options[2].text = "Medio";
            dropdown.options[3].text = "Alto";
            dropdown.options[4].text = "Muy Alto";
            dropdown.options[5].text = "Ultra";
        }

        if (dropdown.options.Count >= 3 && idiomaRuso == true)
        {
            dropdown.options[0].text = "Низкий";
            dropdown.options[1].text = "Очень Низкий";
            dropdown.options[2].text = "Средний";
            dropdown.options[3].text = "Высокий";
            dropdown.options[4].text = "Очень Высокий";
            dropdown.options[5].text = "Ультра";
        }

        if (dropdown.options.Count >= 3 && idiomaAleman == true)
        {
            dropdown.options[0].text = "Niedrig";
            dropdown.options[1].text = "Sehr Niedrig";
            dropdown.options[2].text = "Mittel";
            dropdown.options[3].text = "Hoch";
            dropdown.options[4].text = "Sehr Hoch";
            dropdown.options[5].text = "Ultra";
        }

        if (dropdown.options.Count >= 3 && idiomaItaliano == true)
        {
            dropdown.options[0].text = "Basso";
            dropdown.options[1].text = "Molto Basso";
            dropdown.options[2].text = "Medio";
            dropdown.options[3].text = "Alto";
            dropdown.options[4].text = "Molto Alto";
            dropdown.options[5].text = "Ultra";
        }

        if (dropdown.options.Count >= 3 && idiomaPortugues == true)
        {
            dropdown.options[0].text = "Baixo";
            dropdown.options[1].text = "Muito Baixo";
            dropdown.options[2].text = "Médio";
            dropdown.options[3].text = "Alto";
            dropdown.options[4].text = "Muito Alto";
            dropdown.options[5].text = "Ultra";
        }
    }

    public void confirmarIngles()
    {
        idiomaIngles = true;
        idiomaEspañol = false;
        idiomaRuso = false;
        idiomaAleman = false;
        idiomaItaliano = false;
        idiomaPortugues = false;
        GuardarIdiomaEnPlayerPrefs();
        AsignarNombresDeTextos();
    }

    public void confirmarEspañol()
    {
        idiomaIngles = false;
        idiomaEspañol = true;
        idiomaRuso = false;
        idiomaAleman = false;
        idiomaItaliano = false;
        idiomaPortugues = false;
        GuardarIdiomaEnPlayerPrefs();
        AsignarNombresDeTextos();
    }

    public void confirmarRuso()
    {
        idiomaIngles = false;
        idiomaEspañol = false;
        idiomaRuso = true;
        idiomaAleman = false;
        idiomaItaliano = false;
        idiomaPortugues = false;
        GuardarIdiomaEnPlayerPrefs();
        AsignarNombresDeTextos();
    }

    public void confirmarAleman()
    {
        idiomaIngles = false;
        idiomaEspañol = false;
        idiomaRuso = false;
        idiomaAleman = true;
        idiomaItaliano = false;
        idiomaPortugues = false;
        GuardarIdiomaEnPlayerPrefs();
        AsignarNombresDeTextos();
    }

    public void confirmarItaliano()
    {
        idiomaIngles = false;
        idiomaEspañol = false;
        idiomaRuso = false;
        idiomaAleman = false;
        idiomaItaliano = true;
        idiomaPortugues = false;
        GuardarIdiomaEnPlayerPrefs();
        AsignarNombresDeTextos();
    }

    public void confirmarPortugues()
    {
        idiomaIngles = false;
        idiomaEspañol = false;
        idiomaRuso = false;
        idiomaAleman = false;
        idiomaItaliano = false;
        idiomaPortugues = true;
        GuardarIdiomaEnPlayerPrefs();
        AsignarNombresDeTextos();
    }

    public void GuardarIdiomaEnPlayerPrefs()
    {
        PlayerPrefs.SetInt("ingles", idiomaIngles ? 1 : 0);
        PlayerPrefs.SetInt("español", idiomaEspañol ? 1 : 0);
        PlayerPrefs.SetInt("ruso", idiomaRuso ? 1 : 0);
        PlayerPrefs.SetInt("aleman", idiomaAleman ? 1 : 0);
        PlayerPrefs.SetInt("italiano", idiomaItaliano ? 1 : 0);
        PlayerPrefs.SetInt("portugues", idiomaPortugues ? 1 : 0);
        PlayerPrefs.Save();
    }
}
