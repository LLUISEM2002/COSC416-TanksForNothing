using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    protected Rigidbody rb;
    public int moveSpeed = 5;
    public int baseRotationSnap = 15;
    public float baseRotationSpeed = 100f;
    public float mantleRotationSpeed = 5f;
    public float shootCooldown = 5f;
    public float ShootForce = 1f;
    protected Transform mantle; // Now private to prevent accidental assignment
    protected float rotationDeltaTime = 0f;
    protected Vector3 targetDirection = Vector3.forward;
    protected float shootDeltaTime = 0f;
    [SerializeField] private GameObject bulletPrefab;


    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody is null in Tank!");
        }

        // Automatically find and assign the mantle (child of this object)
        Transform mantleTransform = transform.Find("Mantle");
        if (mantleTransform != null)
        {
            mantle = mantleTransform;
        }
        else
        {
            Debug.LogWarning($"Mantle not found on {gameObject.name}! Make sure it has a child object named 'Mantle'.");
        }
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
    protected void HandleShootBullet(bool IsShooting)
    {
        if (IsShooting && shootDeltaTime > shootCooldown)
        {
            Bullet.FireBullet(bulletPrefab, transform.position, transform.forward, ShootForce);
            shootDeltaTime = 0;
        }
        shootDeltaTime += Time.deltaTime;
    }
}
