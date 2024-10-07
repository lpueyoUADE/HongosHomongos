using System;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum Direction {
    Up = 0x01, Down = 0x02, Right = 0x04, Left = 0x08
}

[Flags]
public enum CrossDirection {
    UpRight = 0x01, UpLeft = 0x02, DownRight = 0x04, DownLeft = 0x08
}

public class Node : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _isJumpNode = false;
    [SerializeField] private bool _isDropNode = false;
     [SerializeField] private Direction _neighboursDirection;
     [SerializeField] private CrossDirection _neighboursCrossDirection;

    [Header("Status")]
    public List<Node> _neighbours = new();
    public List<GameObject> _charactersInside = new();
    
    public Vector3 NodePosition => transform.position;
    public bool NodeJump => _isJumpNode;
    public bool NodeDrop => _isDropNode;
    public List<Node> Neighbours => _neighbours;

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
    public bool _showDebugMessages = false;
    public LayerMask _nodesMask;
    public bool _showRays = false;
    public bool _showReachRays = false;
    public Color _rayColor = Color.red;
    [Range(.1f, 10)] public float _neighbourDistance = 2.5f;

    private void OnDrawGizmos()
    {
        if ((!_showRays || _neighbours.Count == 0) && !_showReachRays) return;

        Vector3 pos = transform.position;

        if (_showRays && _neighbours.Count > 0) for (int i = 0; i < _neighbours.Count; i++) Debug.DrawRay(pos, (_neighbours[i].NodePosition - NodePosition).normalized * _neighbourDistance, _rayColor);
        if (_showReachRays) 
        {
            if (_neighboursDirection.HasFlag(Direction.Up))
            {
                if (Physics.Raycast(pos, Vector3.up, _neighbourDistance, _nodesMask)) Debug.DrawLine(pos, pos + Vector3.up * _neighbourDistance, Color.green);
                else Debug.DrawLine(pos, pos + Vector3.up * _neighbourDistance, Color.red);
            }

            if (_neighboursDirection.HasFlag(Direction.Down))
            {
                if (Physics.Raycast(pos, Vector3.down, _neighbourDistance, _nodesMask)) Debug.DrawLine(pos, pos + Vector3.down * _neighbourDistance, Color.green);
                else Debug.DrawLine(pos, pos + Vector3.down * _neighbourDistance, Color.red);
            }

            if (_neighboursDirection.HasFlag(Direction.Left))
            {
                if (Physics.Raycast(pos, Vector3.left, _neighbourDistance, _nodesMask)) Debug.DrawLine(pos, pos + Vector3.left * _neighbourDistance, Color.green);
                else Debug.DrawLine(pos, pos + Vector3.left * _neighbourDistance, Color.red);
            }

            if (_neighboursDirection.HasFlag(Direction.Right))
            {
                if (Physics.Raycast(pos, Vector3.right, _neighbourDistance, _nodesMask)) Debug.DrawLine(pos, pos + Vector3.right * _neighbourDistance, Color.green);
                else Debug.DrawLine(pos, pos + Vector3.right * _neighbourDistance, Color.red);
            }

            if (_neighboursCrossDirection.HasFlag(CrossDirection.UpRight))
            {
                if (Physics.Raycast(pos, (Vector3.right + Vector3.up).normalized, _neighbourDistance, _nodesMask)) Debug.DrawLine(pos, pos + (Vector3.right + Vector3.up).normalized * _neighbourDistance, Color.green);
                else Debug.DrawLine(pos, pos + (Vector3.right + Vector3.up).normalized * _neighbourDistance, Color.red);
            }

            if (_neighboursCrossDirection.HasFlag(CrossDirection.UpLeft))
            {
                if (Physics.Raycast(pos, (Vector3.left + Vector3.up).normalized, _neighbourDistance, _nodesMask)) Debug.DrawLine(pos, pos + (Vector3.left + Vector3.up).normalized * _neighbourDistance, Color.green);
                else Debug.DrawLine(pos, pos + (Vector3.left + Vector3.up).normalized * _neighbourDistance, Color.red);
            }

            if (_neighboursCrossDirection.HasFlag(CrossDirection.DownLeft))
            {
                if (Physics.Raycast(pos, (Vector3.left + Vector3.down).normalized, _neighbourDistance, _nodesMask)) Debug.DrawLine(pos, pos + (Vector3.left + Vector3.down).normalized * _neighbourDistance, Color.green);
                else Debug.DrawLine(pos, pos + (Vector3.left + Vector3.down).normalized * _neighbourDistance, Color.red);
            }

            if (_neighboursCrossDirection.HasFlag(CrossDirection.DownRight))
            {
                if (Physics.Raycast(pos, (Vector3.right + Vector3.down).normalized, _neighbourDistance, _nodesMask)) Debug.DrawLine(pos, pos + (Vector3.right + Vector3.down).normalized * _neighbourDistance, Color.green);
                else Debug.DrawLine(pos, pos + (Vector3.right + Vector3.down).normalized * _neighbourDistance, Color.red);
            }
        }
    }

    void GetNeighbour(Vector3 dir)
    {
        RaycastHit hit;
        Color color = Color.yellow;
        if (NodeJump) color = Color.blue;
        if (NodeDrop) color = Color.cyan;

        Debug.DrawRay(NodePosition, dir * _neighbourDistance, color);

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
        if (_neighboursDirection.HasFlag(Direction.Up)) GetNeighbour(Vector3.up);
        if (_neighboursDirection.HasFlag(Direction.Down)) GetNeighbour(Vector3.down);
        if (_neighboursDirection.HasFlag(Direction.Left)) GetNeighbour(Vector3.left);
        if (_neighboursDirection.HasFlag(Direction.Right)) GetNeighbour(Vector3.right);
        if (_neighboursCrossDirection.HasFlag(CrossDirection.UpLeft)) GetNeighbour((Vector3.up + Vector3.left).normalized);
        if (_neighboursCrossDirection.HasFlag(CrossDirection.UpRight)) GetNeighbour((Vector3.up + Vector3.right).normalized);
        if (_neighboursCrossDirection.HasFlag(CrossDirection.DownLeft)) GetNeighbour((Vector3.down + Vector3.left).normalized);
        if (_neighboursCrossDirection.HasFlag(CrossDirection.DownRight)) GetNeighbour((Vector3.down + Vector3.right).normalized);

        if (_showDebugMessages) Debug.Log($"{gameObject.name}: Clearing neighbours list - new count: {_neighbours.Count}");
    }

#endif
}
