using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JamokeController : Tank
{
    private Transform player; // Reference to the player tank
    private MapController mapController; // Reference to the MapController

    [SerializeField] private float offsetDistance = 2.0f; // Fixed offset in front of the player's forward direction

    protected override void Start()
    {
        base.Start(); // Call the Tank class's Start() to assign mantle and rigidbody

        // Find and assign the player tank
        GameObject playerObject = GameObject.FindWithTag("Player");

        mapController = FindObjectOfType<MapController>(); // Find the MapController in the scene
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

    public void OnJamokeDestroyed()
    {
        // Inform the MapController, then destroy Jamoke
        if (mapController != null)
        {
            mapController.OnJamokeDone();
        }
        Destroy(gameObject);
    }
}
