using UnityEngine;

public class MantleController : MonoBehaviour
{
    public Camera mainCamera; // Assign your isometric camera in the inspector
    public float rotationSpeed = 100f;
    public float plainHeight = 1f; // Height of the isometric plane

    void Update()
    {
        RotateMantleTowardsCursor();
    }

    void RotateMantleTowardsCursor()
    {
        // Get mouse position in screen space
        Vector3 mouseScreenPosition = Input.mousePosition;

        // Convert to world position based on an approximation for isometric projection
        Vector3 mouseWorldPosition = ScreenToIsometricWorld(mouseScreenPosition);

        // Compute direction and keep rotation locked on the horizontal plane
        Vector3 direction = (mouseWorldPosition - transform.position);
        direction.y = 0; // Keep rotation flat

        // Apply rotation smoothly
        if (direction.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Smoothly interpolate towards target rotation
            transform.rotation = Quaternion.Slerp(
                transform.rotation, // Current rotation
                targetRotation,     // Target rotation
                rotationSpeed * Time.deltaTime // Interpolation factor (adjustable)
            );
        }
    }

    Vector3 ScreenToIsometricWorld(Vector3 screenPosition)
    {
        // Convert screen position to world using a raycast projection on the fixed game plane
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        Vector3 planePosition = new Vector3(0, plainHeight, 0); // Set Y = 1 for the game plane
        Plane gamePlane = new Plane(Vector3.up, planePosition);

        if (gamePlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return Vector3.zero; // Fallback in case the raycast fails
    }
}
