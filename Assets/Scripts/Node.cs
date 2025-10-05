using UnityEngine;

public class Node
{
    public Vector3 WorldPos;
    // Distance from start node (not estimated)
    public float GCost;
    // Heuristic (estimated) distance to goal node
    public float HCost;
    // Total cost (estimated)
    public float FCost => GCost + HCost;
    public Node Parent;

    public Node(Vector3 worldPos)
    {
        WorldPos = worldPos;
        GCost = float.MaxValue;
    }
}