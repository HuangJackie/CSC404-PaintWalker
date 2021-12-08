using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialMusicManager : MonoBehaviour
{
    // private AudioSource backgroundMusic;
    private static GameObject instance;

    // Start is called before the first frame update
    void Start()
    {
        var currentScene = SceneManager.GetActiveScene();
        var currentSceneName = currentScene.name;
        if (currentSceneName == "Tutorial1")
        {
            DontDestroyOnLoad(gameObject);
        }
        if (currentSceneName == "Tutorial2")
        {
            DontDestroyOnLoad(gameObject);
        }
        if (currentSceneName == "Tutorial15")
        {
            DontDestroyOnLoad(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = gameObject;
        }
       
        else
        {
            Destroy(gameObject);
        }
        // backgroundMusic = GetComponent<AudioSource>();
        // backgroundMusic.Play();
    }

    // Update is called once per frame
    void Update()
    {
    }
}