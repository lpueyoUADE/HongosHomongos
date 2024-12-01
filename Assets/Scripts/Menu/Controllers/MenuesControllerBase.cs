using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuesControllerBase : MonoBehaviour
{
    private void OnEnable() 
    {
        ListenToEvents();
    }

    private void OnDisable() 
    {
        StopListenToEvents();
    }

    private void OnDestroy() 
    {
        StopListenToEvents();
    }

    public abstract void ListenToEvents();
    public abstract void StopListenToEvents();
}
