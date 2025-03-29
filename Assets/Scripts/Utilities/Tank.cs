using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    protected Rigidbody rb;
    public int moveSpeed = 5;
    public int baseRotationSnap = 15;
    public float baseRotationSpeed = 100;
    public float mantleRotationSpeed = 5;
    public float shootCooldown = 1;
    public float shootForce = 1;
    public float bulletLifetime = 10f;

    protected Transform mantle; // Handles turret/cannon rotation
    protected Transform wheels; // Optional: Handles visual movement direction
    protected float rotationDeltaTime = 0;
    protected Vector3 targetDirection = Vector3.forward;
    protected float shootDeltaTime = 0;

    [SerializeField] private GameObject bulletPrefab;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody is null in Tank!");
        }

        // Automatically assign mantle if it exists
        Transform mantleTransform = transform.Find("Mantle");
        if (mantleTransform != null)
        {
            mantle = mantleTransform;
        }
        else
        {
            Debug.LogWarning($"Mantle not found on {gameObject.name}! Make sure it has a child object named 'Mantle'.");
        }

        // Optional: assign wheels only if found (not all tanks have them)
        Transform wheelsTransform = transform.Find("Wheels");
        if (wheelsTransform != null)
        {
            wheels = wheelsTransform;
        }

        shootDeltaTime = shootCooldown;
    }

    protected void HandleMove(float moveInput)
    {
        Vector3 moveDirection = transform.forward * moveInput * moveSpeed;
        rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
    }

    protected void HandleRotateBase(float rotateInput)
    {
        if (rotateInput != 0)
        {
            float rotationAngle = rotateInput * baseRotationSpeed * rotationDeltaTime;
            rotationAngle = Mathf.Round(rotationAngle / baseRotationSnap) * baseRotationSnap;

            if (rotationAngle == 0)
            {
                rotationDeltaTime += Time.deltaTime;
            }
            else
            {
                rotationDeltaTime = 0;
            }

            transform.Rotate(Vector3.up * rotationAngle);

            if (mantle != null)
            {
                mantle.Rotate(Vector3.up * -rotationAngle);
            }
        }

        Quaternion currentRotation = transform.rotation;
        float targetYRotation = Mathf.Round(currentRotation.eulerAngles.y / baseRotationSnap) * baseRotationSnap;
        transform.rotation = Quaternion.Euler(0, targetYRotation, 0);
    }

    protected void HandleRotateMantle(Vector3 direction)
    {
        if (direction.magnitude > 0.01f && mantle != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            mantle.rotation = Quaternion.Slerp(mantle.rotation, targetRotation, mantleRotationSpeed * Time.deltaTime);
        }
    }

    protected void HandleRotateWheels(Vector3 directionOverride)
{
    if (wheels == null) return;

    Vector3 direction = new Vector3(directionOverride.x, 0f, directionOverride.z);

    if (direction.magnitude > 0.1f)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        wheels.rotation = Quaternion.Slerp(wheels.rotation, targetRotation, 10f * Time.deltaTime);
    }
}


    protected void HandleShootBullet(bool IsShooting)
    {
        if (IsShooting && shootDeltaTime > shootCooldown)
        {
            Vector3 SpawnOffset = mantle.forward * 1.5f;
            Vector3 SpawnPosition = transform.position + SpawnOffset;
            Bullet.FireBullet(bulletPrefab, SpawnPosition, mantle.forward, shootForce, bulletLifetime);
            shootDeltaTime = 0;
        }

        shootDeltaTime += Time.deltaTime;
    }
}
