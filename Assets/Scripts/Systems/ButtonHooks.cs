using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonHooks : MonoBehaviour
{
     public void LoadNextScene()
    {
        Debug.Log("Play clicked!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
