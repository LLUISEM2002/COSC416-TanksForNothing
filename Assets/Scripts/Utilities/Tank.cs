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
    public Transform mantle;
    
    protected float rotationDeltaTime = 0f;
    protected Vector3 targetDirection = Vector3.forward;


    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (mantle == null)
        {
            Debug.LogError("Mantle transform not assigned!");
            return;
        }
        if (rb == null)
        {
            Debug.LogError("Rigidbody is null in HandleMove!");
            return;
        }
    }
    protected void HandleMove(float moveInput)
    {
        
        Vector3 moveDirection = transform.forward * moveInput * moveSpeed;
        rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
    }

    protected void HandleRotateBase(float rotateInput)
    {
        // Move tank forward/backward in the direction of wheels
        if (rotateInput != 0)
        {
            float rotationAngle = rotateInput * baseRotationSpeed * rotationDeltaTime;

            rotationAngle = Mathf.Round(rotationAngle / baseRotationSnap) * baseRotationSnap;

            // Update rotationDeltaTime if rotationAngle is zero (i.e., no turning)
            if (rotationAngle == 0)
            {
                rotationDeltaTime += Time.deltaTime;
            }
            else
            {
                rotationDeltaTime = 0;
            }

            transform.Rotate(Vector3.up * rotationAngle);

            mantle.Rotate(Vector3.up * -rotationAngle);

        }

        // Snap the tank based on baseRotationSnap
        Quaternion currentRotation = transform.rotation;
        float targetYRotation = Mathf.Round(currentRotation.eulerAngles.y / baseRotationSnap) * baseRotationSnap;
        
        transform.rotation = Quaternion.Euler(0, targetYRotation, 0);
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
