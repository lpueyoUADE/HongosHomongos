using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> _neighbours = new List<Node>();
    
    public Vector3 NodePosition => transform.position;

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
        GetNeighbour(Vector3.forward);
        GetNeighbour(Vector3.back);
        Debug.Log($"{gameObject.name}: Clearing neighbours list - new count: {_neighbours.Count}");
    }

#endif
}
