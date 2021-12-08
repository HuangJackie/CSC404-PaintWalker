using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicClipSwapTutorial1 : MonoBehaviour
{
    // Start is called before the first frame update

    public AudioSource music;
    public AudioClip tutorial;

    public AudioSource cutsceneMusic;
    public AudioClip cutsceneTutorial;
    // Start is called before the first frame update
    void Start()
    {
        music.clip = tutorial;
        music.Play();

        cutsceneMusic.clip = cutsceneTutorial;
        cutsceneMusic.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}