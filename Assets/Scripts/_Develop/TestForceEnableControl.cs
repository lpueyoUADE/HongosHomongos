using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestForceEnableControl : MonoBehaviour
{
    public BaseCharacter _chScript;
    public BasePlayerControls _playerScript;
    public Camera _camera;

    private void Awake()
    {
        _chScript= GetComponent<BaseCharacter>();
    }

    private void Start()
    {
        _chScript.InControl(true);
        _playerScript._camera = _camera;
    }
}
