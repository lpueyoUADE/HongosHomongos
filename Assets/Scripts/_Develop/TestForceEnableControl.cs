using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestForceEnableControl : MonoBehaviour
{
    public BaseCharacter _chScript;

    private void Awake()
    {
        _chScript= GetComponent<BaseCharacter>();
    }

    private void Start()
    {
        _chScript.InControl(true);
    }
}
