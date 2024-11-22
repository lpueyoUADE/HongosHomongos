using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNavMeshNavigationButtons : MonoBehaviour
{
    public BaseCharacterControl characterInControl;
    public BaseCharacter playerTarget;

    public void WalkTowards()
    {
        Vector3 destination = playerTarget.transform.position;
        destination.z = -0.5038481f; // the fuck
        characterInControl.NavAgentSetDestination(destination);
    }

    public void WalkAway()
    {
        
    }

    public void Stop()
    {
        characterInControl.NavAgentForceStop();
    }
}
