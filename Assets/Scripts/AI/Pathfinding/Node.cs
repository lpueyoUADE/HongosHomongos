using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _isJumpNode = false;
    [SerializeField] private bool _isDropNode = false;

    [Header("Status")]
    public List<Node> _neighbours = new();
    public List<GameObject> _charactersInside = new();
    
    public Vector3 NodePosition => transform.position;
    public bool NodeJump => _isJumpNode;
    public bool NodeDrop => _isDropNode;


    private void Awake() 
    {
        if (_isJumpNode) _isDropNode = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Character")) return;
        _charactersInside.Add(other.gameObject);

        if (_isJumpNode) PathfindingEvents.OnNodeJumpUpdate?.Invoke(other.gameObject, true);
        if (_isDropNode) PathfindingEvents.OnNodeDropUpdate?.Invoke(other.gameObject, true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Character")) return;
        _charactersInside.Remove(other.gameObject);

        if (_isJumpNode) PathfindingEvents.OnNodeJumpUpdate?.Invoke(other.gameObject, false);
        if (_isDropNode) PathfindingEvents.OnNodeDropUpdate?.Invoke(other.gameObject, false);
    }

    public Vector3 GetCharacterPosition(GameObject character)
    {
        if (_charactersInside.Contains(character)) return _charactersInside[_charactersInside.IndexOf(character)].transform.position;
        else return Vector3.zero;
    }

#if UNITY_EDITOR
    public LayerMask _nodesMask;
    public bool _showRays = false;
    public Color _rayColor = Color.red;
    [Range(.1f, 5)] public float _neighbourDistance = 2.5f;

    private void OnDrawGizmos()
    {
        if (!_showRays || _neighbours.Count == 0) return;

        for (int i = 0; i < _neighbours.Count; i++)
            Debug.DrawRay(transform.position, (_neighbours[i].transform.position - NodePosition).normalized * _neighbourDistance, _rayColor);
    }

    void GetNeighbour(Vector3 dir)
    {
        RaycastHit hit;

        Debug.DrawRay(NodePosition, dir * _neighbourDistance, Color.yellow);

        if (Physics.Raycast(NodePosition, dir, out hit, _neighbourDistance, _nodesMask))
        {
            var node = hit.collider.GetComponent<Node>();
            if (node && !_neighbours.Contains(node)) _neighbours.Add(node);
        }
    }

    [ContextMenu("Get Neighbours")]
    public void GetNeighbours()
    {
        _neighbours.Clear();
        GetNeighbour(Vector3.right);
        GetNeighbour(Vector3.left);
        GetNeighbour(Vector3.up);
        GetNeighbour(Vector3.down);
        Debug.Log($"{gameObject.name}: Clearing neighbours list - new count: {_neighbours.Count}");
    }

#endif
}
