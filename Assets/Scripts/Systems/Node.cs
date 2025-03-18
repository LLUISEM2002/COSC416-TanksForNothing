using UnityEngine;

public class Node
{
    public bool walkable;  // Can the tank walk on this node?
    public Vector3 worldPosition;  // World space position of the node
    public int gridX, gridZ;  // Position in the grid

    public int gCost;  // Cost from start node
    public int hCost;  // Estimated cost to end node
    public Node parent;  // Reference to parent node in path

    public int fCost => gCost + hCost;  // Total cost

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridZ)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridZ = gridZ;
    }
}