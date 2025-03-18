using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JamokeController : MonoBehaviour
{
    private Transform player; // Reference to the player tank
    private Transform mantle; // Reference to the mantle (auto-assigned)

    [SerializeField] private float offsetDistance = 2.0f; // Fixed offset in front of the player's forward direction
    [SerializeField] private float mantleRotationSpeed = 5.0f; // Speed of mantle rotation

    void Start()
    {
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

        // Find and assign the mantle (child of this object)
        Transform mantleTransform = transform.Find("Mantle");
        if (mantleTransform != null)
        {
            mantle = mantleTransform;
        }
        else
        {
            Debug.LogWarning("Mantle not found! Make sure your enemy tank has a child object named 'Mantle'.");
        }
    }

    void Update()
    {
        if (player != null && mantle != null)
        {
            RotateMantleTowardsOffsetPosition();
        }
    }

    void RotateMantleTowardsOffsetPosition()
    {
        // Fixed offset in front of the player's forward direction
        Vector3 targetPosition = player.position + (player.forward * offsetDistance);

        // Velocity based Offset
        /*
        if (playerRb != null)
        {
            targetPosition += playerRb.velocity * predictionMultiplier; // Offset based on velocity
        }
        */

        // Calculate the direction to the target position
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // Keep rotation on the horizontal plane

        HandleRotateMantle(direction);
    }

    protected void HandleRotateMantle(Vector3 direction)  
    {
        if (direction.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            // Smoothly interpolate towards target rotation
            mantle.rotation = Quaternion.Slerp(
                mantle.rotation,
                targetRotation,
                mantleRotationSpeed * Time.deltaTime
            );
        }
    }
}
