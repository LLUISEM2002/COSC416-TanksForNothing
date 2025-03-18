using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlitzController : Tank
{
    private Transform player; // Reference to the player tank
    private TankPathFollower pathFollower; // Handles movement along path
    private AStarPathfinding pathfinding; // Reference to pathfinding system

    [SerializeField] private float offsetDistance = 2.0f; // How far ahead of the player to aim
    [SerializeField] private float repathRate = 2.0f; // How often to recalculate path
    private float nextPathTime = 0f; // Timer for repathing

    protected override void Start()
    {
        base.Start(); // Initialize rigidbody and mantle from Tank class

        // Find the player
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player tank not found! Make sure it has the 'Player' tag.");
        }

        // Get references to necessary components
        pathFollower = GetComponent<TankPathFollower>();
        pathfinding = FindFirstObjectByType<AStarPathfinding>();

        if (pathFollower == null)
        {
            Debug.LogError("TankPathFollower component is missing on Blitz!");
        }
        if (pathfinding == null)
        {
            Debug.LogError("AStarPathfinding component is missing in the scene!");
        }
    }

    void Update()
    {
        if (player != null)
        {
            // Periodically request a new path to the player
            if (Time.time >= nextPathTime)
            {
                RequestPathToPlayer();
                nextPathTime = Time.time + repathRate;
            }

            // Continue rotating the mantle towards the player while moving
            RotateMantleTowardsPlayer();
        }
    }

    void RequestPathToPlayer()
    {
        if (pathfinding != null && pathFollower != null)
        {
            pathFollower.MoveToTarget(player.position); // Request path to player
        }
    }

    void RotateMantleTowardsPlayer()
    {
        if (player == null) return;

        // Predict the player's movement based on forward direction
        Vector3 targetPosition = player.position + (player.forward * offsetDistance);
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // Keep rotation on the horizontal plane

        HandleRotateMantle(direction); // Uses Tank's method to rotate the mantle
    }
}
