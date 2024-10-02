using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingEvents
{
    private static NodeGridManager _gridManager;
    public static Action<GameObject, bool> OnNodeJumpUpdate;
    public static Action<GameObject, bool> OnNodeDropUpdate;

    public static void UpdateGridManager(NodeGridManager gridManager)
    {
        _gridManager = gridManager;
    }

    public static List<Node> GetPath(Vector3 startingPoint, Transform objective)
    {
        return _gridManager.RunAStarPlus(startingPoint, objective);
    }

    public static List<Node> GetPath(Transform startingObject, Transform objective) 
    { 
        return _gridManager.RunAStarPlus(startingObject, objective);    
    }

    public static List<Node> GetRandomReachableNodes(GameObject startingObject)
    {
        return _gridManager.GetReachableNodes(startingObject);
    }

    public static List<Node> GetRandomReachableNodesAwayFrom(GameObject startingObject, GameObject evadeObject, float minDistance = 25)
    {
        return _gridManager.GetReachableNodesInEvadingDistance(startingObject, evadeObject, minDistance);
    }

    public static Node GetRandomNode()
    {
        return _gridManager.RandomNode;
    }
}
