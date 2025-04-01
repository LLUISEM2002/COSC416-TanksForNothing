using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JamokeController : Tank
{
    private Transform player;

    [SerializeField] private float offsetDistance = 2.0f;

    [SerializeField] private float initialShootDelay = 1f;
    private float timeSinceSpawn = 0f;

    [SerializeField] private LayerMask wallAndPlayerMask;

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

        timeSinceSpawn += Time.deltaTime;

        RotateMantleTowardsOffsetPosition();

        // Wait before allowing the Jamoke to shoot
        if (timeSinceSpawn < initialShootDelay) return;

        shootDeltaTime += Time.deltaTime;

        if (shootDeltaTime > shootCooldown)
        {
            if (HasLineOfSightToPlayer() && IsMantleAimedAtPlayer())
            {
                Debug.Log("Jamoke shooting!");
                HandleShootBullet(true);
                shootDeltaTime = 0f;
            }
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

    bool HasLineOfSightToPlayer()
    {
        Vector3 origin = Mantle.position;
        Vector3 direction = (player.position - origin).normalized;
        float distance = Vector3.Distance(origin, player.position);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, wallAndPlayerMask))
        {
            bool hitPlayer = false;

            Transform current = hit.transform;
            while (current != null)
            {
                if (current.CompareTag("Player"))
                {
                    hitPlayer = true;
                    break;
                }
                current = current.parent;
            }

            Debug.DrawLine(origin, hit.point, hitPlayer ? Color.red : Color.blue, 0.1f);
            return hitPlayer;
        }

        // Raycast didn’t hit anything at all — draw a gray line
        Debug.DrawLine(origin, origin + direction * distance, Color.gray, 0.1f);
        return false;
    }

    bool IsMantleAimedAtPlayer()
    {
        if (Mantle == null) return false;

        Vector3 toPlayer = (player.position - Mantle.position).normalized;
        Vector3 flatForward = new Vector3(Mantle.forward.x, 0, Mantle.forward.z).normalized;
        Vector3 flatTarget = new Vector3(toPlayer.x, 0, toPlayer.z).normalized;

        float angle = Vector3.Angle(flatForward, flatTarget);
        return angle < 5f;
    }
}
