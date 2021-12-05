using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foosteps : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] clips;
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
    }

    private void Step()
    {
        AudioClip clip = GetRandomClip();
        audioSource.PlayOneShot(clip);
    }

    private AudioClip GetRandomClip()
    {
        return clips[UnityEngine.Random.Range(0, clips.Length)];

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
