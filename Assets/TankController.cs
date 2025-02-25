using UnityEngine;

public class TankController : MonoBehaviour
{
    public float moveSpeed = 5f;  
    public float rotationSpeed = 100f;
    private Rigidbody rb;

    void Start()
    {
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
