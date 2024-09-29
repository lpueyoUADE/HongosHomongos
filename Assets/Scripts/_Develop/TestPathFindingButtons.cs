using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestPathFindingButtons : MonoBehaviour
{    
    public TestPathFinding _aiController;

    public void TestPath()
    {
        _aiController.MakeNewPath();
    }
}
