using UnityEngine;

public class MantleController : MonoBehaviour
{
    public Camera mainCamera; // Assign your isometric camera in the inspector
    public RenderTexture lowResTexture; // Assign the lower-resolution texture from the camera

    private float rotationSpeed;
    private float textureScaleFactor; // Scale factor for adjusting input coordinates
    private float plainHeight;

    void Start()
    {
        rotationSpeed = GameManager.instance.playerSettings.mantleRotationSpeed;
        plainHeight = GameManager.instance.playerSettings.mantlePlaneHeight;

        // Calculate scale factor based on actual screen resolution and low-res texture
        if (lowResTexture != null)
        {
            textureScaleFactor = (float)Screen.width / lowResTexture.width;
        }
        else
        {
            Debug.LogWarning("Low-resolution texture not assigned! Using default scale.");
            textureScaleFactor = 1f; // Default to no scaling if no texture is provided
        }
    }

    void Update()
    {
        RotateMantleTowardsCursor();
    }

    void RotateMantleTowardsCursor()
    {
        if (mainCamera == null)
        {
            Debug.LogWarning("Main camera not assigned!");
            return;
        }

        // Get mouse position and scale it to match the low-res texture
        Vector3 mousePosition = Input.mousePosition / textureScaleFactor;

        Vector3 mouseWorldPosition = ScreenToIsometricWorld(mousePosition);

        Vector3 direction = (mouseWorldPosition - transform.position);
        direction.y = 0; // Keep rotation flat

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

// TODO: Fix this for when implementing bullet generation so that the angle is accurate