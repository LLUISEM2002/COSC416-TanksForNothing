using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private Camera mainCamera;

    public bool IsShooting { get; private set; }
    public Vector3 MouseWorldPosition { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Update()
    {
        IsShooting = Input.GetMouseButton(0);

        Vector3 mouseScreenPos = Input.mousePosition;
        MouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0f));
        // MouseWorldPosition.z = 0f; // Optional: lock to 2D
    }

    public float GetMoveInput()
    {
        return Input.GetAxis("Vertical");
    }

    public float GetTurnInput()
    {
        return Input.GetAxis("Horizontal");
    }
}
