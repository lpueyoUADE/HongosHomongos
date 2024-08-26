using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class logicaPantallaCompleta : MonoBehaviour
{
    public Toggle toggle;

    public TMP_Dropdown resolucionesDropdown;

    Resolution[] resoluciones;

    void Start()
    {
        bool pantallaCompletaGuardada = PlayerPrefs.GetInt("pantallaCompleta", 1) == 1;
        toggle.isOn = pantallaCompletaGuardada;

        RevisarResolucion();
    }

    public void activarPantallaCompleta(bool pantallaCompleta)
    {
        PlayerPrefs.SetInt("pantallaCompleta", pantallaCompleta ? 1 : 0);
        Screen.fullScreen = pantallaCompleta;
    }

    public void RevisarResolucion()
    {
        resoluciones = Screen.resolutions;
        resolucionesDropdown.ClearOptions();
        List<string> opciones = new List<string>();
        int resolucionActual = 0;

        for (int i = 0; i < resoluciones.Length; i++)
        {
            string opcion = resoluciones[i].width + " x " + resoluciones[i].height;
            opciones.Add(opcion);


            if (Screen.fullScreen && resoluciones[i].width == Screen.currentResolution.width &&
                resoluciones[i].height == Screen.currentResolution.height)
            {
                resolucionActual = i;
            }

        }

        resolucionesDropdown.AddOptions(opciones);
        resolucionesDropdown.value = resolucionActual;
        resolucionesDropdown.RefreshShownValue();



        resolucionesDropdown.value = PlayerPrefs.GetInt("numeroResolucion", 0);

    }

    public void CambiarResolucion(int indiceResolucion)
    {
        PlayerPrefs.SetInt("numeroResolucion", resolucionesDropdown.value);



        Resolution resolution = resoluciones[indiceResolucion];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}

