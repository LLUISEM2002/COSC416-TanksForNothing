using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton for global access

    [Header("Player Settings")] // Main category
    public PlayerSettings playerSettings;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

[System.Serializable] // Makes this class visible in the Inspector
public class PlayerSettings
{
    [Header("Tank Settings")]
    public float tankMoveSpeed = 5f;
    public float tankRotationSpeed = 100f;
    public int rotationSnap = 10;

    [Header("Mantle Settings")]
    public float mantleRotationSpeed = 5f;
    public float mantlePlaneHeight = 1f; 
}