using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JamokeController : MonoBehaviour
{
    private Transform player; // Reference to the player tank
    private Rigidbody playerRb; // Reference to the player's Rigidbody for velocity tracking
    private Transform mantle; // Reference to the mantle (auto-assigned)

    [SerializeField] private float predictionMultiplier = 1.5f; // Adjust how far ahead the enemy aims

    void Start()
    {
        // Find and assign the player tank
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerRb = playerObject.GetComponent<Rigidbody>(); // Get the player's Rigidbody
            if (playerRb == null)
            {
                Debug.LogWarning("Player Rigidbody not found! Predictive aiming will be less accurate.");
            }
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
            AimAtPredictedPlayerPosition();
        }
    }

    void AimAtPredictedPlayerPosition()
    {
        // Get the player's velocity and predict future position
        Vector3 predictedPosition = player.position;

        if (playerRb != null)
        {
            predictedPosition += playerRb.linearVelocity * predictionMultiplier; // Offset based on velocity
        }

        // Calculate the direction to the predicted position
        Vector3 direction = predictedPosition - transform.position;
        direction.y = 0; // Keep the rotation on the horizontal plane

        // Rotate the mantle toward the predicted position
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        mantle.rotation = Quaternion.Slerp(mantle.rotation, targetRotation, Time.deltaTime * 5f); // Adjust speed as needed
    }
}
