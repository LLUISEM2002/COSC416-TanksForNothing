using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 20;  // Number of nodes in the X direction
    public int gridHeight = 20; // Number of nodes in the Z direction
    public float nodeSize = 1f; // Distance between nodes
    public LayerMask obstacleLayer; // Layer that determines non-walkable areas

    private Node[,] grid;
    private Vector3 gridOrigin; // The bottom-left corner of the grid

    void Start()
    {
        CalculateGridOrigin(); // Find the true starting position
        CreateGrid();
    }

    void CalculateGridOrigin()
    {
        // The grid center is where GameManager is, so we calculate bottom-left corner
        gridOrigin = transform.position - new Vector3(gridWidth * nodeSize / 2, 0, gridHeight * nodeSize / 2);
    }

    void CreateGrid()
    {
        grid = new Node[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                // Calculate the correct world position considering the center shift
                Vector3 worldPoint = gridOrigin + new Vector3(x * nodeSize, 0, z * nodeSize);
                
                // Check if this point is walkable (doesn't overlap with obstacles)
                bool walkable = !Physics.CheckSphere(worldPoint, nodeSize / 2, obstacleLayer);
                
                // Create and store the node
                grid[x, z] = new Node(walkable, worldPoint, x, z);
            }
        }
    }

    public Node GetNodeFromWorldPosition(Vector3 worldPosition)
    {
        // Convert the world position to a local grid position
        int x = Mathf.RoundToInt((worldPosition.x - gridOrigin.x) / nodeSize);
        int z = Mathf.RoundToInt((worldPosition.z - gridOrigin.z) / nodeSize);
        
        // Ensure we don't access out-of-bounds indices
        return grid[Mathf.Clamp(x, 0, gridWidth - 1), Mathf.Clamp(z, 0, gridHeight - 1)];
    }

    void OnDrawGizmos()
    {
        if (grid == null) return; // Ensure the grid exists before drawing

        foreach (Node node in grid)
        {
            if (node == null) continue; // Avoid null reference errors

            // Set color based on whether the node is walkable or not
            Gizmos.color = node.walkable ? Color.green : Color.red;

            // Draw a cube in the XZ plane with a slight Y offset so it’s visible
            Vector3 gizmoPosition = new Vector3(node.worldPosition.x, 0.1f, node.worldPosition.z);
            Gizmos.DrawCube(gizmoPosition, new Vector3(nodeSize * 0.9f, 0.1f, nodeSize * 0.9f)); // Shrink for visibility
        }
    }
}
