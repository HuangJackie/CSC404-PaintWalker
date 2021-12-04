using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutSceneManager : MonoBehaviour
{
    private List<GameObject> _cutSceneStorage;
    private LevelManager _levelManager;
    private CutSceneDontDeleteManager _cutSceneDontDestroyManager;
    private bool _levelLoadComplete;
    private static GameObject instance; 
    // Start is called before the first frame update

    private void Awake()
    {
        
    }
    void Start()
    {
        instance = gameObject;
        _levelManager = FindObjectOfType<LevelManager>();
        _cutSceneDontDestroyManager = FindObjectOfType<CutSceneDontDeleteManager>();
        if (!_cutSceneDontDestroyManager.cutScenesSeen)
        {
            _cutSceneStorage = new List<GameObject>();
            int children = transform.childCount;
            for (int i = children - 1; i > -1; --i)
            {
                if (transform.GetChild(i).gameObject.activeSelf)
                {
                    _cutSceneStorage.Add(transform.GetChild(i).gameObject);
                }
            }
            _cutSceneDontDestroyManager.cutScenesSeen = true;
        } else
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_cutSceneStorage.Count > 0)
        {
            _levelManager.freezePlayer = true;
            if (Input.GetMouseButtonDown(0) && _levelLoadComplete)
            {
                GameObject curCutScene = _cutSceneStorage[0];
                _cutSceneStorage.RemoveAt(0);
                curCutScene.SetActive(false);
            }
        } else
        {
            _levelManager.freezePlayer = false;
            this.gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        _levelLoadComplete = true;
    }

    public bool ShowingCutscenes()
    {
        return _cutSceneStorage != null && _cutSceneStorage.Count > 0;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Level Loaded");
        Debug.Log(scene.name);
        Debug.Log(mode);
    }
}
