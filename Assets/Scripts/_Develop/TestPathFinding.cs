using System.Collections.Generic;
using UnityEngine;

public class TestPathFinding : MonoBehaviour
{
    public BaseCharacter _target;
    private BaseCharacter _testingCharacter;

    public BaseCharacter AICharacter => _testingCharacter;
    public List<Node> _path = new();

    private void Awake() 
    {
        _testingCharacter = GetComponent<BaseCharacter>();
    }

    public void MakeNewPath()
    {
        _path = PathfindingEvents.GetPath(_testingCharacter.transform, _target.transform);
    }

    private void Update() 
    {
        if (_path.Count == 0) return;

        for (int i = 0; i < _path.Count; i++)
        {
            if (i == _path.Count - 1) break;
            Debug.DrawLine(_path[i].NodePosition, _path[i + 1].NodePosition, Color.red);
        }
    }
}
