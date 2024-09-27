using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGridManager : MonoBehaviour
{
    public static List<Node> _nodes = new List<Node>();
    public LayerMask _nodeMask;
    public float _nearNodeRadius = 3;
    public Node _targetNode;

    private void Awake()
    {
        _nodes.Clear();
        foreach (Transform child in transform) _nodes.Add(child.GetComponent<Node>());
    }

    // Pathfinding using node
    public List<Node> RunAStarPlus(Vector3 startingPosition, Node objetive)
    {
        if (startingPosition == Vector3.zero || _targetNode == null) return null;

        _targetNode = GetCloserNodeToNode(objetive);
        var start = GetNearNode(startingPosition);

        if (!_targetNode || !start) return null;

        List<Node> path = AStar.Run(start, objetive, GetConnections, IsSatisfies, GetCost, Heuristic);
        path = AStar.CleanPath(path, InView);

        _targetNode = null;
        return path;
    }

    // Agent controller functions
    private List<Node> GetConnections(Node current)
    {
        return current._neighbours;
    }

    private Node GetNearNode(Vector3 startingPosition)
    {
        var nodes = Physics.OverlapSphere(startingPosition, _nearNodeRadius, _nodeMask);
        Node nearNode = null;
        float nearDistance = 0;

        for (int i = 0; i < nodes.Length; i++)
        {
            var currentNode = nodes[i];
            var dir = currentNode.transform.position - startingPosition;
            float currentDistance = dir.magnitude;

            if (nearNode == null || currentDistance < nearDistance)
            {
                if (!Physics.Raycast(startingPosition, dir.normalized, currentDistance, _nodeMask))
                {
                    nearNode = currentNode.GetComponent<Node>();
                    nearDistance = currentDistance;
                }
            }
        }

        return nearNode;
    }

    private bool IsSatisfies(Node current)
    {
        return current == _targetNode;
    }

    private bool InView(Node grandParent, Node child)
    {
        return InView(grandParent.NodePosition, child.NodePosition);
    }

    private bool InView(Vector3 a, Vector3 b)
    {
        Vector3 dir = b - a;

#if UNITY_EDITOR
        if (!Physics.Raycast(a, dir.normalized, dir.magnitude, _nodeMask)) Debug.DrawLine(a, b, Color.white);
        else Debug.DrawLine(a, b, Color.red);
#endif

        return !Physics.Raycast(a, dir.normalized, dir.magnitude, _nodeMask);
    }

    private float Heuristic(Node current)
    {
        float heuristic = 0;
        float multiplierDistance = 1;
        heuristic += Vector3.Distance(current.NodePosition, _targetNode.NodePosition) * multiplierDistance;

        return heuristic;
    }

    private float GetCost(Node parent, Node child, Node target)
    {
        float cost = 0;
        float multiplierDistance = 1;
        cost += Vector3.Distance(parent.NodePosition, child.NodePosition) * multiplierDistance;

        return cost;
    }

    // Static agent controller functions
    
    public static Node GetCloserNodeToNode(Node target)
    {
        if (_nodes.Count == 0 || !target) return null;
        return null;
    }


#if UNITY_EDITOR
    [ContextMenu("Refresh nodes")]
    void RefreshNodeList()
    {
        _nodes.Clear();
        foreach (Transform child in transform) _nodes.Add(child.GetComponent<Node>());
    }

    [ContextMenu("Refresh nodes and get neighbourds")]
    void RefreshNodeListAndGetNeighbourds()
    {
        _nodes.Clear();
        foreach (Transform child in transform) _nodes.Add(child.GetComponent<Node>());
        foreach (Node node in _nodes) node.GetNeighbours();
    }
#endif
}
