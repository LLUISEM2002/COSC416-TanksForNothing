using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankPathFollower : MonoBehaviour
{
    private AStarPathfinding pathfinder; // Make it private to prevent manual assignment
    public float speed = 3f;

    private List<Node> path;
    private int pathIndex;

    void Start()
    {
        GameObject gameManager = GameObject.FindWithTag("GameManager");
        if (gameManager != null)
        {
            pathfinder = gameManager.GetComponent<AStarPathfinding>();
        }

        if (pathfinder == null)
        {
            Debug.LogError("TankPathFollower: AStarPathfinding component not found! Ensure GameManager has the AStarPathfinding script attached.");
        }
    }

    public void MoveToTarget(Vector3 targetPosition)
    {
        if (pathfinder == null)
        {
            Debug.LogError("TankPathFollower: Cannot find pathfinder. Ensure it's assigned.");
            return;
        }

        path = pathfinder.FindPath(transform.position, targetPosition);
        pathIndex = 0;
        StopAllCoroutines();
        StartCoroutine(FollowPath());
    }

    IEnumerator FollowPath()
    {
        while (pathIndex < path.Count)
        {
            Vector3 targetPos = path[pathIndex].worldPosition;

            // Ensure the tank only moves on XZ plane, keeping the original Y
            Vector3 movePosition = new Vector3(targetPos.x, transform.position.y, targetPos.z);

            while (Vector3.Distance(transform.position, movePosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, movePosition, speed * Time.deltaTime);
                yield return null;
            }
            pathIndex++;
        }
    }
}
