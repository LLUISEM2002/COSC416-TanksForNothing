using UnityEngine;

public class ButtonHooks : MonoBehaviour
{
    public void LoadNextScene()
    {
        MapController map = FindObjectOfType<MapController>(); // Get instance in the scene
        if (map != null)
        {
            map.GoToNextMap("map1");
        }
        else
        {
            Debug.LogError("MapController not found in the scene!");
        }
    }
}
