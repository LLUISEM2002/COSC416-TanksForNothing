using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    protected Rigidbody rb;
    public int moveSpeed = 5;
    public float rotationSpeed = 100f;
    public int rotationSnap = 15;
    public Transform mantle;

    protected float rotationDeltaTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (mantle == null)
        {
            Debug.LogError("Mantle transform not assigned!");
        }
    }
    protected void handleMove(float moveInput)
    {
        Vector3 moveDirection = transform.forward * moveInput * moveSpeed;
        rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
    }
    protected void handleTurn(float turnInput)
    {
        // Move tank forward/backward in the direction of wheels
        if (turnInput != 0)
        {
            float rotationAngle = turnInput * rotationSpeed * rotationDeltaTime;

            rotationAngle = Mathf.Round(rotationAngle / rotationSnap) * rotationSnap;

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

        // Snap the tank based on rotationSnap
        Quaternion currentRotation = transform.rotation;
        float targetYRotation = Mathf.Round(currentRotation.eulerAngles.y / rotationSnap) * rotationSnap;
        
        transform.rotation = Quaternion.Euler(0, targetYRotation, 0);
    }
}
