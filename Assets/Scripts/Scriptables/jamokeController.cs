using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JamokeController : Tank
{
    private Transform player;

    [SerializeField] private float offsetDistance = 2.0f;

    [Header("Shooting Settings")]
    [SerializeField] private float fireRate = 2f; // Seconds between shots
    private float shootTimer = 0f;

    protected override void Awake()
    {
        base.Awake();

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
        if (player == null) return;

        RotateMantleTowardsOffsetPosition();

        shootDeltaTime += Time.deltaTime;

        if (shootDeltaTime > shootCooldown)
        {
            Debug.Log("Jamoke shooting!");
            HandleShootBullet(true); 
            shootDeltaTime = 0f;
        }
        else
        {
            float timeLeft = shootCooldown - shootDeltaTime;
            Debug.Log($"Jamoke cooldown: {timeLeft:F2} seconds remaining");
        }
    }


    void RotateMantleTowardsOffsetPosition()
    {
        Vector3 targetPosition = player.position + (player.forward * offsetDistance);
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;

        HandleRotateMantle(direction);
    }

    void TryShootAtMantleDirection()
    {
        // Only try to shoot if Mantle is assigned and roughly aimed at the player
        if (Mantle != null)
        {
            HandleShootBullet(true);
        }
    }
}
