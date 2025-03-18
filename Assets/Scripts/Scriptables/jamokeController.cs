using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JamokeController : MonoBehaviour
{
    private Transform player; // Reference to the player tank
    private Transform mantle; // Reference to the mantle (auto-assigned)

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
            AimAtPlayer();
        }
    }

    void AimAtPlayer()
    {
        // Calculate the direction to the player
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Keep the rotation on the horizontal plane

        // Rotate the mantle toward the player
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        mantle.rotation = Quaternion.Slerp(mantle.rotation, targetRotation, Time.deltaTime * 5f); // Adjust speed as needed
    }
}

