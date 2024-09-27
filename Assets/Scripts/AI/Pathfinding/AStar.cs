using System;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    // Use target reference
    public static List<T> Run<T>(T start, Transform target, Func<T, List<T>> getConnections, Func<T, bool> isSatisfies, Func<T, T, Transform, float> getCost, Func<T, float> heuristic, int watchdog = 500)
    {
        PriorityQueue<T> pending = new();
        HashSet<T> visited = new();
        Dictionary<T, T> parents = new();
        Dictionary<T, float> cost = new();

        pending.Enqueue(start, 0);
        cost[start] = 0;

        while (!pending.IsEmpty)
        {
            // Don't run forever
            watchdog--; if (watchdog <= 0) break;

            T current = pending.Dequeue();

            if (isSatisfies(current))
            {
                var path = new List<T> { current };
                while (parents.ContainsKey(path[path.Count - 1])) path.Add(parents[path[path.Count - 1]]);
                path.Reverse();
                return path;
            }

            visited.Add(current);
            List<T> connections = getConnections(current);

            for (int i = 0; i < connections.Count; i++)
            {
                T child = connections[i];
                if (visited.Contains(child)) continue;

                float currentCost = cost[current] + getCost(current, child, target);
                if (cost.ContainsKey(child) && currentCost >= cost[child]) continue;

                cost[child] = currentCost;
                pending.Enqueue(child, currentCost + heuristic(child));
                parents[child] = current;
            }
        }

        return new List<T>();
    }

    // Use node reference
    public static List<T> Run<T>(T start, T node, Func<T, List<T>> getConnections, Func<T, bool> isSatisfies, Func<T, T, T, float> getCost, Func<T, float> heuristic, int watchdog = 500)
    {
        PriorityQueue<T> pending = new();
        HashSet<T> visited = new();
        Dictionary<T, T> parents = new();
        Dictionary<T, float> cost = new();

        pending.Enqueue(start, 0);
        cost[start] = 0;

        while (!pending.IsEmpty)
        {
            // Don't run forever
            watchdog--; if (watchdog <= 0) break;

            T current = pending.Dequeue();

            if (isSatisfies(current))
            {
                var path = new List<T> { current };

                while (parents.ContainsKey(path[path.Count - 1])) path.Add(parents[path[path.Count -1]]);
                path.Reverse();
                return path;
            }

            visited.Add(current);
            List<T> connections = getConnections(current);

            for (int i = 0; i < connections.Count; i++)
            {
                T child = connections[i];
                if (visited.Contains(child)) continue;

                float currentCost = cost[current] + getCost(current, child, node);
                if (cost.ContainsKey(child) && currentCost >= cost[child]) continue;

                cost[child] = currentCost;
                pending.Enqueue(child, currentCost + heuristic(child));
                parents[child] = current;
            }
        }

        return new List<T>();
    }

    // Others
    public static List<T> CleanPath<T>(List<T> path, Func<T ,T, bool> inView)
    {
        if (path == null || path.Count <= 2) return path;

        var newPath = new List<T> { path[0] };

        for (int i = 0; i < path.Count; i++)
        {
            var gp = newPath[newPath.Count - 1];
            if (!inView(gp, path[i])) newPath.Add(path[i - 1]);
        }

        newPath.Add(path[path.Count - 1]);
        return newPath;
    }
}
