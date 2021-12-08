using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneMusicManager : MonoBehaviour
{
    // private AudioSource backgroundMusic;
    private static GameObject instance;

    // Start is called before the first frame update
    void Awake()
    {
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
