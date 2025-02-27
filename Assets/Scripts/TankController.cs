using UnityEngine;

public class TankController : MonoBehaviour
{
    private Rigidbody rb;
    private float moveSpeed; 
    private float rotationSpeed;

    void Start()
    {
        moveSpeed = GameManager.instance.playerSettings.tankMoveSpeed;
        rotationSpeed = GameManager.instance.playerSettings.tankRotationSpeed;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float moveInput = InputManager.instance.GetMoveInput();
        float turnInput = InputManager.instance.GetTurnInput();

        // Move tank forward/backward in the direction of wheels
        Vector3 moveDirection = transform.forward * moveInput * moveSpeed;
        rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);

        // Rotate the tank (wheels steering logic)
        if (turnInput != 0)
        {
            transform.Rotate(Vector3.up * turnInput * rotationSpeed * Time.deltaTime);
        }
    }
}
