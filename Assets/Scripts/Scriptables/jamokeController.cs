using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JamokeController : Tank
{
    private Transform player; // Reference to the player tank

    [SerializeField] private float offsetDistance = 2.0f; // Fixed offset in front of the player's forward direction

    protected override void Awake()
    {
        base.Awake(); // Call the Tank class's Start() to assign mantle and rigidbody

        // Find and assign the player tank
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player tank not found! Make sure it has the 'Player' tag.");
        }
    }

    void Update()
    {
        if (player != null)
        {
            RotateMantleTowardsOffsetPosition();
        }
    }

    void RotateMantleTowardsOffsetPosition()
    {
        Vector3 targetPosition = player.position + (player.forward * offsetDistance);
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;

        HandleRotateMantle(direction);
    }
}
