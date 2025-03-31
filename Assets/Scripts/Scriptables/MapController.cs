using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    public Animator transitionAnim;
    private int jamokeCount;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Called each time a scene finishes loading
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Recalculate how many jamokes are in the new scene
        GameObject[] jamokeObjects = GameObject.FindGameObjectsWithTag("Tank");
        jamokeCount = jamokeObjects.Length;
        Debug.Log("Scene loaded: " + scene.name + " - Jamoke count: " + jamokeCount);
    }

    // Called from your Jamoke when it's destroyed
    public void OnJamokeDone()
    {
        jamokeCount--;
        Debug.Log("Jamoke destroyed! Remaining jamokes: " + jamokeCount);

        if (jamokeCount <= 0)
        {
             if (SceneManager.GetActiveScene().buildIndex == 5)
            {
                // We are on the last map (Map5). 
                // No more transitions. Possibly show a "Victory" message or do nothing.
                Debug.Log("All jamokes destroyed on final map! Game Over / Victory!");
                SceneManager.LoadScene(6);
            }else{
                Debug.Log("All jamokes destroyed! Loading next level...");
                StartCoroutine(LoadNextLevel(SceneManager.GetActiveScene().buildIndex + 1));
            }
            
        }
    }

    // Public method if you want to load next scene by name
    public void GoToNextMap(string nextMapName)
    {
        StartCoroutine(LoadNextLevel(nextMapName));
    }

    private IEnumerator LoadNextLevel(string sceneNameOrIndex)
    {
        // 1) Fade Out
        transitionAnim.SetTrigger("End");  // "End" triggers fade‐out animation
        yield return new WaitForSeconds(1f);  // Wait length of fade

        // 2) Load by name
        SceneManager.LoadScene(sceneNameOrIndex);

        // 3) Fade In
        transitionAnim.SetTrigger("Start");  // "Start" triggers fade‐in animation
    }

    // Overload for int index if you prefer
    private IEnumerator LoadNextLevel(int sceneBuildIndex)
    {
        // 1) Fade Out
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1f);

        // 2) Load scene by index
        SceneManager.LoadScene(sceneBuildIndex);

        // 3) Fade In
        transitionAnim.SetTrigger("Start");
    }
}

