using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicClipSwapTutorial15 : MonoBehaviour
{
    public AudioSource music;
    public AudioClip tutorial;
    // Start is called before the first frame update
    void Start()
    {
        music.clip = tutorial;
        music.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
