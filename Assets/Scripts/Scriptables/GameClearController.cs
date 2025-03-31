using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class GameClearController : MonoBehaviour
{
    public Animator transitionAnim;
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
}
