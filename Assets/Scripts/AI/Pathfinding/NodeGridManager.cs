using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NodeGridManager : MonoBehaviour
{
    public static List<Node> _nodes = new List<Node>();
    public float _nearNodeRadius = 3;
    public Node _targetNode;

    [Tooltip("For InView check - this needs to be collision, anything that can block the path between nodes.")] 
    public LayerMask _nodeMask;

    private void Awake()
    {
        PathfindingEvents.UpdateGridManager(this);
        _nodes.Clear();
        foreach (Transform child in transform) _nodes.Add(child.GetComponent<Node>());
    }

    // Pathfinding
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
    
    public List<Node> RunAStarPlus(Vector3 startingPosition, Transform targetObjective)
    {
        _targetNode = GetCloserNodeToTarget(targetObjective.gameObject);
        var start = GetNearNode(startingPosition);

        if (!_targetNode || !start) return null;

        List<Node> path = AStar.Run(start, _targetNode, GetConnections, IsSatisfies, GetCost, Heuristic);
        path = AStar.CleanPath(path, InView);
        return path;
    }

    public List<Node> RunAStarPlus(Transform startingObject, Transform targetObjective)
    {
        _targetNode = GetCloserNodeToTarget(targetObjective.gameObject);
        var start = GetCloserNodeToTarget(startingObject.gameObject);
        if (!_targetNode || !start) return null;

        List<Node> path = AStar.Run(start, _targetNode, GetConnections, IsSatisfies, GetCost, Heuristic);
        path = AStar.CleanPath(path, InView);
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

    // Disabled for now
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

    public static Node GetCloserNodeToTarget(GameObject target)
    {
        Node result = null;

        // Check first the nodes that may have the target.
        for (int i = 0; i < _nodes.Count; i++) if (_nodes[i].GetCharacterPosition(target) != Vector3.zero) break;

        // If no result then check by distance
        if (!result)
        {
            Vector3 targetPosition = target.transform.position;
            float distance = Vector3.Distance(_nodes[0].NodePosition, targetPosition);
            result = _nodes[0];

            for (int i = 1; i < _nodes.Count; i++)
            {
                float newDistance = Vector3.Distance(_nodes[i].NodePosition, targetPosition);

                if (newDistance < distance)
                {
                    distance = newDistance;
                    result = _nodes[i];
                }
            }
        }

        return result;
    }

#if UNITY_EDITOR
    [Header("Editor only")]
    public bool _refreshOnStartIfEditor = true;

    private void Start() 
    {
        if (!_refreshOnStartIfEditor) return;
        RefreshNodeListAndGetNeighbourds();
    }

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
