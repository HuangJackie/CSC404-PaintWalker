using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicClipSwapLevel2 : MonoBehaviour
{

    public AudioSource music;
    public AudioClip level2;

    // Start is called before the first frame update
    void Start()
    {
        music.clip = level2 ;
        music.Play();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
