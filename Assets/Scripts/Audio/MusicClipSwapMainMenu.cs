using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicClipSwapMainMenu : MonoBehaviour
{
    public AudioSource music;
    public AudioClip mainmenu;
    // Start is called before the first frame update
    void Start()
    {
        music.clip = mainmenu;
        music.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
