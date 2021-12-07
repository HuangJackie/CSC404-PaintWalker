using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartDontDeleteManager : MonoBehaviour
{
    private static GameObject instance;
    public bool isRestarting;
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
