using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneDontDeleteManager : MonoBehaviour
{
    private static GameObject instance;
    public bool cutScenesSeen;
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
