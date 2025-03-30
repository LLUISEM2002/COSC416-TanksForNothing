using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlitzController : Tank
{
    private Transform player;
    private TankPathFollower pathFollower;
    private AStarPathfinding pathfinding;

    [SerializeField] private float repathRate = 2.0f;
    private float nextPathTime = 0f;

    [Header("Bouncing Shot Settings")]
    [SerializeField] private LayerMask wallAndPlayerMask;
    [SerializeField] private int maxBounces = 3;
    [SerializeField] private float maxRayDistance = 100f;

    [Header("Projectile Settings")]
    [SerializeField] private GameObject bounceBulletPrefab;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float fireCooldown = 2f;
    private float fireCooldownTimer = 0f;

    private Vector3 currentBounceDirection;
    private bool pendingFire = false;

    protected override void Awake()
    {
        base.Awake();

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        pathFollower = GetComponent<TankPathFollower>();
        pathfinding = FindFirstObjectByType<AStarPathfinding>();
    }

    void Update()
    {
        if (player != null)
        {
            if (pathFollower != null && pathFollower.CurrentVelocity != Vector3.zero)
            {
                HandleRotateWheels(pathFollower.CurrentVelocity);
            }

            if (Time.time >= nextPathTime)
            {
                RequestPathToPlayer();
                nextPathTime = Time.time + repathRate;
            }

            if (fireCooldownTimer <= 0f)
            {
                AimWithBounceFan();
            }

            if (currentBounceDirection != Vector3.zero)
            {
                HandleRotateMantle(currentBounceDirection);

                if (pendingFire && IsMantleAimed(currentBounceDirection))
                {
                    FireBouncingBullet(currentBounceDirection);
                    pendingFire = false;
                    fireCooldownTimer = fireCooldown;
                }
            }

            fireCooldownTimer -= Time.deltaTime;
        }
    }

    void RequestPathToPlayer()
    {
        if (pathFollower != null && pathfinding != null)
        {
            pathFollower.MoveToTarget(player.position);
        }
    }

    void AimWithBounceFan()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 baseDirection = (player.position - transform.position).normalized;

        float[] angles = { -90f, -75f, -60f, -45f, -30f, -15f, 0f, 15f, 30f, 45f, 60f, 75f, 90f };

        foreach (float angle in angles)
        {
            Vector3 rotatedDirection = Quaternion.Euler(0, angle, 0) * baseDirection;

            var (hitPlayer, path) = GetReflectedPath(origin, rotatedDirection, maxBounces, maxRayDistance, wallAndPlayerMask);

            for (int i = 0; i < path.Count - 1; i++)
            {
                Color rayColor = hitPlayer ? Color.red : Color.cyan;
                Debug.DrawLine(path[i], path[i + 1], rayColor, 1.1f);
                Debug.DrawRay(path[i], Vector3.up * 0.5f, Color.yellow, 1.1f);
            }

            if (hitPlayer && path.Count >= 2)
            {
                currentBounceDirection = path[1] - path[0];
                currentBounceDirection.y = 0;
                pendingFire = true;
                return;
            }
        }

        currentBounceDirection = Vector3.zero;
        pendingFire = false;
    }

    bool IsMantleAimed(Vector3 targetDirection)
    {
        if (Mantle == null) return false;

        Vector3 flatForward = new Vector3(Mantle.forward.x, 0, Mantle.forward.z).normalized;
        Vector3 flatTarget = targetDirection.normalized;

        float angle = Vector3.Angle(flatForward, flatTarget);
        return angle < 3f;
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

                Transform colliderTransform = hit.collider.transform;
                while (colliderTransform != null)
                {
                    if (colliderTransform.CompareTag("Player"))
                    {
                        return (true, points);
                    }
                    colliderTransform = colliderTransform.parent;
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
        if (bounceBulletPrefab != null && Mantle != null)
        {
            Vector3 spawnOffset = shootDirection.normalized * 1.5f;
            Vector3 spawnPosition = transform.position + spawnOffset;

            Bullet.FireBullet(bounceBulletPrefab, spawnPosition, shootDirection, bulletSpeed, bulletLifetime, maxBounces);
        }
    }
}
