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
    public float shootForce = 10;
    public int bulletMaxBounce = 3;
    public float bulletLifetime = 10f;

    protected Transform mantle; // private use
    public Transform Mantle { get; protected set; } // public access
    protected Transform wheels;

    protected float rotationDeltaTime = 0;
    protected Vector3 targetDirection = Vector3.forward;
    protected float shootDeltaTime = 0;

    [SerializeField] private GameObject bulletPrefab;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody is null in Tank!");
        }

        Transform mantleTransform = transform.Find("Mantle");
        if (mantleTransform != null)
        {
            mantle = mantleTransform;
            Mantle = mantleTransform;
        }
        else
        {
            Debug.LogWarning($"Mantle not found on {gameObject.name}! Make sure it has a child object named 'Mantle'.");
        }

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

            if (Mantle != null)
            {
                Mantle.Rotate(Vector3.up * -rotationAngle);
            }
        }

        Quaternion currentRotation = transform.rotation;
        float targetYRotation = Mathf.Round(currentRotation.eulerAngles.y / baseRotationSnap) * baseRotationSnap;
        transform.rotation = Quaternion.Euler(0, targetYRotation, 0);
    }

    protected void HandleRotateMantle(Vector3 direction)
    {
        if (direction.magnitude > 0.01f && Mantle != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Mantle.rotation = Quaternion.Slerp(Mantle.rotation, targetRotation, mantleRotationSpeed * Time.deltaTime);
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
        if (IsShooting && shootDeltaTime > shootCooldown && Mantle != null)
        {
            Vector3 spawnOffset = Mantle.forward * 1.5f;
            Vector3 spawnPosition = transform.position + spawnOffset;
            Bullet.FireBullet(bulletPrefab, spawnPosition, Mantle.forward, shootForce, bulletLifetime, bulletMaxBounce);
            shootDeltaTime = 0;
        }

        shootDeltaTime += Time.deltaTime;
    }
}
