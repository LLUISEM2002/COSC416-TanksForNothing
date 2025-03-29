using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance; // Singleton pattern for global access

    public bool IsShooting {get; private set; }
    public bool ShowRay {get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public float GetMoveInput()
    {
        return Input.GetAxis("Vertical"); // W/S or Up/Down
    }

    public float GetTurnInput()
    {
        return Input.GetAxis("Horizontal"); // A/D or Left/Right
    }
    void Update()
    {
        IsShooting = Input.GetMouseButton(0); // left mouse button
        if (Input.GetKeyDown("e"))
        {
            ShowRay = !ShowRay;
        }
    }
}