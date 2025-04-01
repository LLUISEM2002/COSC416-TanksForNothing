using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonHooks : MonoBehaviour
{
     public void LoadNextScene()
    {
        Debug.Log("Play clicked!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadAgain()
    {
    Debug.Log("Play Again clicked!");
    int mapIndex = MapController.lastMapIndex;
    if (mapIndex >= 0)
    {
        SceneManager.LoadScene(mapIndex);
    }
    else
    {
        Debug.LogWarning("lastMapIndex not set. Returning to Main Menu instead.");
        SceneManager.LoadScene(0);
    }
    }

    public void LoadMainMenu()
    {
        Debug.Log("Main Menu clicked!");
        SceneManager.LoadScene(0);
    }
}
