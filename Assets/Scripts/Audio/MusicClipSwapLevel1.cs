using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicClipSwapLevel1 : MonoBehaviour
{
    public AudioSource music;
    public AudioClip level1;
    // Start is called before the first frame update
    void Start()
    {
        music.clip = level1;
        music.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
