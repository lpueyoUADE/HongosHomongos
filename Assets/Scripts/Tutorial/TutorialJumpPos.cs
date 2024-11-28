using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialJumpPos : MonoBehaviour
{
    public GameObject player;

    private void OnTriggerEnter(Collider other) 
    {
        if (!other.gameObject != player) return;
        TutorialControls.OnJumpReached?.Invoke();
        Destroy(gameObject);
    }
}
