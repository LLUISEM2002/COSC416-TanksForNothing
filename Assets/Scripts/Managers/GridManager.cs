using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 20;
    public int gridHeight = 20;
    public float nodeSize = 1f;
    public LayerMask obstacleLayer;

    private Node[,] grid;
    private Vector3 gridOrigin;

    void Start()
    {
        CalculateGridOrigin();
        CreateGrid();
    }

    void CalculateGridOrigin()
    {
        gridOrigin = transform.position - new Vector3(gridWidth * nodeSize / 2, 0, gridHeight * nodeSize / 2);
    }

    void CreateGrid()
    {
        grid = new Node[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 worldPoint = gridOrigin + new Vector3(x * nodeSize, 0, z * nodeSize);
                bool walkable = !Physics.CheckSphere(worldPoint, nodeSize / 2, obstacleLayer);
                grid[x, z] = new Node(walkable, worldPoint, x, z);
            }
        }
    }

    public Node GetNodeFromWorldPosition(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt((worldPosition.x - gridOrigin.x) / nodeSize);
        int z = Mathf.RoundToInt((worldPosition.z - gridOrigin.z) / nodeSize);
        return grid[Mathf.Clamp(x, 0, gridWidth - 1), Mathf.Clamp(z, 0, gridHeight - 1)];
    }

    // ✅ NEW METHOD: Get Neighbors of a Given Node
    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                // Skip the current node itself
                if (x == 0 && z == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkZ = node.gridZ + z;

                // Ensure the node is within grid bounds
                if (checkX >= 0 && checkX < gridWidth && checkZ >= 0 && checkZ < gridHeight)
                {
                    neighbors.Add(grid[checkX, checkZ]);
                }
            }
        }
        return neighbors;
    }

    void OnDrawGizmos()
    {
        if (grid == null) return;

        foreach (Node node in grid)
        {
            if (node == null) continue;
            Gizmos.color = node.walkable ? Color.green : Color.red;
            Vector3 gizmoPosition = new Vector3(node.worldPosition.x, 0.1f, node.worldPosition.z);
            Gizmos.DrawCube(gizmoPosition, new Vector3(nodeSize * 0.9f, 0.1f, nodeSize * 0.9f));
        }
    }
}
