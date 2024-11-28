using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestForceEnableControl : MonoBehaviour
{
    public BaseCharacter _chScript;
    public BasePlayerControls _playerScript;
    public TutorialPlayerControls _tutScript;
    public Camera _camera;

    private void Awake()
    {
        _chScript = GetComponent<BaseCharacter>();
    }

    private void Start()
    {
        _chScript?.InControl(true);
        if (_playerScript) _playerScript._camera = _camera;
        if (_tutScript) _tutScript._camera = _camera;
    }
}
