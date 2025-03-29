// using System.Collections;
// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class SceneController : MonoBehaviour
// {
//     [SerializeField] private Animator fadeAnimator; // Animator for fade animation
//     [SerializeField] private float fadeDuration = 1f; // Duration of the fade animation

//     private bool isTransitioning = false;

//     // Call this method when all enemy tanks are destroyed
//     public void LoadNextStage(string nextSceneName)
//     {
//         if (!isTransitioning)
//         {
//             StartCoroutine(TransitionToNextStage(nextSceneName));
//         }
//     }

//     private IEnumerator TransitionToNextStage(string nextSceneName)
//     {
//         isTransitioning = true;

//         // Trigger the fade-out animation
//         fadeAnimator.SetTrigger("FadeOut");

//         // Wait for the fade animation to complete
//         yield return new WaitForSeconds(fadeDuration);

//         // Load the next scene
//         SceneManager.LoadScene(nextSceneName);

//         isTransitioning = false;
//     }
// }