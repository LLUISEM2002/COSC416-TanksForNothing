using UnityEngine;

public class MantleController : MonoBehaviour
{
    public Camera mainCamera; // Assign your isometric camera in the inspector
    public RenderTexture lowResTexture; // Assign the lower-resolution texture from the camera

    private Plane groundPlane;
    private float rotationSpeed;
    private Vector2 textureScaleFactor; // Scale factor for adjusting input coordinates

    void Start()
    {
        groundPlane = new Plane(Vector3.up, Vector3.zero);
        
        rotationSpeed = GameManager.instance.playerSettings.mantleRotationSpeed;

        // Calculate scale factor based on actual screen resolution and low-res texture
        if (lowResTexture != null)
        {
            textureScaleFactor = new Vector2(
                (float)Screen.width / lowResTexture.width,
                (float)Screen.height / lowResTexture.height
            );
        }
        else
        {
            Debug.LogWarning("Low-resolution texture not assigned! Using default scale.");
            textureScaleFactor = Vector2.one; // Default to no scaling if no texture is provided
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

        Vector3 mousePosition = Input.mousePosition;
        mousePosition.x /= textureScaleFactor.x;
        mousePosition.y /= textureScaleFactor.y;

        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 intersectionPoint = ray.GetPoint(enter);

            Vector3 direction = intersectionPoint - transform.position;
            direction.y = 0; // Keep rotation on the horizontal plane

            if (direction.magnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                // Smoothly interpolate towards target rotation
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }
}
