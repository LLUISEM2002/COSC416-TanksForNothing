using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlitzController : Tank
{
    private Transform player;
    private TankPathFollower pathFollower;
    private AStarPathfinding pathfinding;

    [SerializeField] private float offsetDistance = 2.0f;
    [SerializeField] private float repathRate = 2.0f;
    private float nextPathTime = 0f;

    [Header("Bouncing Shot Settings")]
    [SerializeField] private LayerMask wallAndPlayerMask;
    [SerializeField] private int maxBounces = 3;
    [SerializeField] private float maxRayDistance = 100f;
    [SerializeField] private float aimInterval = 1.0f;
    private float nextAimTime = 0f;

    [Header("Projectile Settings")]
    [SerializeField] private GameObject bounceBulletPrefab;
    [SerializeField] private float bulletSpeed = 20f;

    private Vector3 currentBounceDirection;
    private bool pendingFire = false;

    protected override void Start()
    {
        base.Start();

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            Debug.Log("Blitz is targeting: " + player.name + " | Root: " + player.root.name + " | Tag: " + player.tag + " | Layer: " + LayerMask.LayerToName(player.gameObject.layer));
        }
        else
        {
            Debug.LogWarning("Player tank not found! Make sure it has the 'Player' tag.");
        }

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
            if (Time.time >= nextPathTime)
            {
                RequestPathToPlayer();
                nextPathTime = Time.time + repathRate;
            }

            if (Time.time >= nextAimTime)
            {
                AimWithBounce();
                nextAimTime = Time.time + aimInterval;
            }

            // Continuously rotate mantle to follow bounce direction
            if (currentBounceDirection != Vector3.zero)
            {
                HandleRotateMantle(currentBounceDirection);

                // Check if mantle is now aligned and we have a shot pending
                if (pendingFire && IsMantleAimed(currentBounceDirection))
                {
                    FireBouncingBullet(currentBounceDirection);
                    pendingFire = false;
                }
            }
        }
    }

    void RequestPathToPlayer()
    {
        if (pathfinding != null && pathFollower != null)
        {
            Debug.Log("Pathfinding temporarily disabled for aiming debug.");
            // pathFollower.MoveToTarget(player.position);
        }
    }

    void AimWithBounce()
    {
        Debug.Log("Attempting to aim and fire...");
        Vector3 start = transform.position + Vector3.up * 0.5f;
        Vector3 direction = (player.position - transform.position).normalized;

        var (hitPlayer, bouncePath) = GetReflectedPath(start, direction, maxBounces, maxRayDistance, wallAndPlayerMask);

        for (int i = 0; i < bouncePath.Count - 1; i++)
        {
            Debug.DrawLine(bouncePath[i], bouncePath[i + 1], Color.cyan, 1.1f);
            Debug.DrawRay(bouncePath[i], Vector3.up * 0.5f, Color.yellow, 1.1f);
        }

        if (bouncePath.Count >= 2)
        {
            currentBounceDirection = bouncePath[1] - bouncePath[0];
            currentBounceDirection.y = 0;

            if (hitPlayer)
            {
                Debug.Log("Player detected! Will fire once mantle aligns...");
                pendingFire = true;
            }
            else
            {
                Debug.Log("Ray did NOT hit player.");
                pendingFire = false;
            }
        }
        else
        {
            Debug.Log("Insufficient bounce points to determine aim.");
            currentBounceDirection = Vector3.zero;
            pendingFire = false;
        }
    }

    bool IsMantleAimed(Vector3 targetDirection)
    {
        if (mantle == null) return false;

        Vector3 flatForward = new Vector3(mantle.forward.x, 0, mantle.forward.z).normalized;
        Vector3 flatTarget = targetDirection.normalized;

        float angle = Vector3.Angle(flatForward, flatTarget);
        return angle < 3f; // within 3 degrees is "close enough"
    }

    (bool, List<Vector3>) GetReflectedPath(Vector3 startPos, Vector3 direction, int maxBounces, float maxDistance, LayerMask mask)
    {
        List<Vector3> points = new List<Vector3> { startPos };
        Vector3 currentPosition = startPos;
        Vector3 currentDirection = direction;

        for (int i = 0; i < maxBounces; i++)
        {
            Ray ray = new Ray(currentPosition, currentDirection);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance, mask))
            {
                points.Add(hit.point);

                // Traverse up the hierarchy to find a parent with the "Player" tag
                Transform playerTransform = hit.collider.transform;
                while (playerTransform != null && !playerTransform.CompareTag("Player"))
                {
                    playerTransform = playerTransform.parent;
                }

                Debug.Log("Ray hit: " + hit.collider.name + " | Tag: " + hit.collider.tag +
                          " | Matching parent (Player?): " + playerTransform?.name + " | Tag: " + playerTransform?.tag);

                if (playerTransform != null)
                {
                    Debug.Log("Ray hit the player!");
                    return (true, points); // Stop bouncing
                }

                currentDirection = Vector3.Reflect(currentDirection, hit.normal);
                currentPosition = hit.point;
            }
            else
            {
                points.Add(currentPosition + currentDirection * maxDistance);
                break;
            }
        }

        return (false, points);
    }

    void FireBouncingBullet(Vector3 shootDirection)
    {
        if (bounceBulletPrefab != null && mantle != null)
        {
            Vector3 spawnOffset = shootDirection.normalized * 1.5f;
            Vector3 spawnPosition = transform.position + spawnOffset;

            Debug.Log("Firing bullet from Blitz in bounce direction!");
            Bullet.FireBullet(bounceBulletPrefab, spawnPosition, shootDirection, bulletSpeed, bulletLifetime);
        }
        else
        {
            Debug.LogWarning("Missing bounceBulletPrefab or mantle.");
        }
    }
}
