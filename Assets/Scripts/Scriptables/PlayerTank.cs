using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTank : Tank
{
    private Camera mainCamera; // Assign your isometric camera in the inspector
    [SerializeField] private RenderTexture lowResTexture; // Assign the lower-resolution texture from the camera
    private Plane groundPlane;
    private Vector2 textureScaleFactor; // Scale factor for adjusting input coordinates

    protected override void Start()
    {
        base.Start();
        mainCamera = Camera.main;
        groundPlane = new Plane(Vector3.up, Vector3.zero);


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
        float moveInput = InputManager.instance.GetMoveInput();
        float rotateInput = InputManager.instance.GetTurnInput();
        HandleInput(moveInput, rotateInput);
        RotateMantleTowardsCursor();
    }

    void HandleInput(float moveInput, float rotateInput)
    {
        HandleMove(moveInput);
        HandleRotateBase(rotateInput);
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
