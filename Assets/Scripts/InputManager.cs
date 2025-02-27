using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance; // Singleton pattern for global access

    private void Awake()
    {
        if (instance == null)
            instance = this;
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
}