using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestPathFindingButtons : MonoBehaviour
{    
    public TestPathFinding _aiController;

    public void MakeTowardPath()
    {
        _aiController.MakeNewPathTowards();
    }

    public void MakeAwayPath()
    {
        _aiController.MakeNewPathAway();
    }

    public void WalkTowards()
    {
        _aiController.Walk(true);
    }

        public void WalkAway()
    {
        _aiController.Walk(false);
    }
}
