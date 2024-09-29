using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingEvents
{
    private static NodeGridManager _gridManager;

    public static void UpdateGridManager(NodeGridManager gridManager)
    {
        _gridManager = gridManager;
    }

    public static List<Node> GetPath(Vector3 startingPoint, Node objective)
    {
        return _gridManager.RunAStarPlus(startingPoint, objective);
    }

    public static List<Node> GetPath(Vector3 startingPoint, Transform objective)
    {
        return _gridManager.RunAStarPlus(startingPoint, objective);
    }

    public static List<Node> GetPath(Transform startingObject, Transform objective) 
    { 
        return _gridManager.RunAStarPlus(startingObject, objective);    
    }
}
