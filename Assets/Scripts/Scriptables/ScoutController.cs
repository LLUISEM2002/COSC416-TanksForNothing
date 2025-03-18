using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutController : MonoBehaviour
{
    private Transform player; // Reference to the player tank
    private Transform mantle; // Reference to the mantle (auto-assigned)
    [SerializeField] private float mantleRotationSpeed = 5.0f; // Speed at which the mantle rotates

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
        Vector3 targetPosition = player.position;

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
