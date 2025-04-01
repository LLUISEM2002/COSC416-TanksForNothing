using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [System.Serializable]
    public class StageAudio
    {
        public List<string> sceneNames;
        public AudioSource audioSource;
    }

    public List<StageAudio> stageAudios = new List<StageAudio>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool foundMatch = false;

        foreach (var stageAudio in stageAudios)
        {
            if (stageAudio.sceneNames.Contains(scene.name))
            {
                if (stageAudio.audioSource != null)
                {
                    // If this track is already playing, do nothing
                    if (stageAudio.audioSource.isPlaying)
                    {
                        Debug.Log("Audio already playing for this scene: " + scene.name);
                        return;
                    }

                    // Stop all others and play this one
                    StopAllAudioExcept(stageAudio.audioSource);
                    stageAudio.audioSource.Play();
                    Debug.Log("Started new track for scene: " + scene.name);
                    foundMatch = true;
                    break;
                }
            }
        }

        if (!foundMatch)
        {
            // No audio match for this scene, stop all
            StopAllAudioExcept(null);
            Debug.Log("No matching audio found for scene: " + scene.name);
        }
    }

    void StopAllAudioExcept(AudioSource exception)
    {
        foreach (var stageAudio in stageAudios)
        {
            if (stageAudio.audioSource != null && stageAudio.audioSource != exception)
            {
                stageAudio.audioSource.Stop();
            }
        }
    }


    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
