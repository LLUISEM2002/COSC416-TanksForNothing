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
    public Transform Mantle { get; protected set; }
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

        Mantle = transform.Find("Mantle");
        if (Mantle == null)
        {
            Debug.LogWarning($"Mantle not found on {gameObject.name}! Make sure it has a child object named 'Mantle'.");
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
    protected void HandleShootBullet(bool IsShooting)
    {
        if (IsShooting && shootDeltaTime > shootCooldown)
        {
            Vector3 SpawnOffset = Mantle.forward * 1.5f; // Adjust 1.5f to push it farther or closer
            Vector3 SpawnPosition = transform.position + SpawnOffset;
            Bullet.FireBullet(bulletPrefab, SpawnPosition, Mantle.forward, shootForce, bulletLifetime);
            shootDeltaTime = 0;
        }
        shootDeltaTime += Time.deltaTime;
    }
}
