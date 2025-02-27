using UnityEngine;

public class TankController : MonoBehaviour
{
    private Rigidbody rb;
    private float moveSpeed; 
    private float rotationSpeed;
    private float rotationSnap;
    private float rotationDeltaTime;

    public Transform mantle;

    void Start()
    {
        moveSpeed = GameManager.instance.playerSettings.tankMoveSpeed;
        rotationSpeed = GameManager.instance.playerSettings.tankRotationSpeed;
        rotationSnap = GameManager.instance.playerSettings.rotationSnap;
        rb = GetComponent<Rigidbody>();
        rotationDeltaTime = 0;

        // Ensure the mantle reference is set
        if (mantle == null)
        {
            Debug.LogError("Mantle transform not assigned!");
        }
    }

    void Update()
    {
        float moveInput = InputManager.instance.GetMoveInput();
        float turnInput = InputManager.instance.GetTurnInput();

        // Move tank forward/backward in the direction of wheels
        Vector3 moveDirection = transform.forward * moveInput * moveSpeed;
        rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);

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
        Quaternion currentRotation = transform.rotation;
        float targetYRotation = Mathf.Round(currentRotation.eulerAngles.y / rotationSnap) * rotationSnap;
        transform.rotation = Quaternion.Euler(0, targetYRotation, 0);
    }
}
