using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceSoundManager : MonoBehaviour
{
    // private AudioSource ambience;
    private static GameObject instance;

    // Start is called before the first frame update
    void Start()
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
        // ambience = GetComponent<AudioSource>();
        // ambience.Play();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
