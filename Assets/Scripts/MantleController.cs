using UnityEngine;

public class MantleController : MonoBehaviour
{
    public Camera mainCamera; // Assign your isometric camera in the inspector
    public RenderTexture lowResTexture; // Assign the lower-resolution texture from the camera

    private float rotationSpeed;
    private float textureScaleFactor; // Scale factor for adjusting input coordinates

    void Start()
    {
        rotationSpeed = GameManager.instance.playerSettings.mantleRotationSpeed;

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

        // Convert screen position to world position using a raycast
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        Plane gamePlane = new Plane(Vector3.up, transform.position); // Assume mantle is on the ground plane

        if (gamePlane.Raycast(ray, out float enter))
        {
            Vector3 mouseWorldPosition = ray.GetPoint(enter);

            // Compute direction in XZ plane only
            Vector3 direction = new Vector3(mouseWorldPosition.x - transform.position.x, 0, mouseWorldPosition.z - transform.position.z);

            if (direction.magnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                
                // Smooth rotation around Y-axis
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}

// TODO: Fix this for when implementing bullet generation so that the angle is accurate