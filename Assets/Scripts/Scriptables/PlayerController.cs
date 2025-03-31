using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Tank
{
    private Camera mainCamera; // Assign your isometric camera in the inspector
    [SerializeField] private RenderTexture lowResTexture; // Assign the lower-resolution texture from the camera
    private Plane groundPlane;
    private Vector2 textureScaleFactor; // Scale factor for adjusting input coordinates

    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
        groundPlane = new Plane(Vector3.up, new Vector3(0,0.65f,0));


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

        shootForce = 10f;
    }
    void Update()
    {
        float MoveInput = InputManager.Instance.GetMoveInput();
        float RotateInput = InputManager.Instance.GetTurnInput();
        bool IsShooting = InputManager.Instance.IsShooting;
        HandleInput(MoveInput, RotateInput, IsShooting);
        RotateMantleTowardsCursor();
    }

    void HandleInput(float moveInput, float rotateInput, bool IsShooting)
    {
        HandleMove(moveInput);
        HandleRotateBase(rotateInput);
        HandleShootBullet(IsShooting);
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

            HandleRotateMantle(direction);
        }
    }
}
