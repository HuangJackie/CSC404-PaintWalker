using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DefaultNamespace;

public class CutSceneManager : MonoBehaviour
{
    private List<GameObject> _cutSceneStorage;
    private List<GameObject> _endSceneStorage;
    private List<GameObject> _currScenes;
    private LevelManager _levelManager;
    private CutSceneDontDeleteManager _cutSceneDontDestroyManager;
    private ControllerUtil _controllerUtil;
    private bool endSceneActive;

    private static GameObject instance;
    private bool _levelLoadComplete;

    void Start()
    {
        instance = gameObject;
        _levelManager = FindObjectOfType<LevelManager>();
        _cutSceneDontDestroyManager = FindObjectOfType<CutSceneDontDeleteManager>();
        _controllerUtil = FindObjectOfType<ControllerUtil>();
        _cutSceneStorage = new List<GameObject>();
        _endSceneStorage = new List<GameObject>();
        _currScenes = new List<GameObject>();

        if (!_cutSceneDontDestroyManager.cutScenesSeen)
        {
            int children = transform.childCount;

            for (int i = children - 1; i > -1; --i)
            {
                if (transform.GetChild(i).gameObject.activeSelf && !transform.GetChild(i).gameObject.name.Contains("End"))
                {
                    _cutSceneStorage.Add(transform.GetChild(i).gameObject);
                } else if (transform.GetChild(i).gameObject.name.Contains("End"))
                {
                    _endSceneStorage.Add(transform.GetChild(i).gameObject);
                    transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            _cutSceneDontDestroyManager.cutScenesSeen = true;
        }
        else if (_cutSceneDontDestroyManager.endCutScenesSeen)
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {

        if (_cutSceneStorage.Count > 0)
        {
            _currScenes = _cutSceneStorage;
        } 

        if (_endSceneStorage.Count > 0 && endSceneActive)
        {
            _currScenes = _endSceneStorage;
        }
        
        if (_currScenes != null) {
            if (_currScenes.Count > 0)
            {
                _levelManager.freezePlayer = true;
                if ((Input.GetMouseButtonDown(0) ||
                     _controllerUtil.GetConfirmButtonPressed() ||
                     _controllerUtil.GetCancelButtonPressed()) &&
                    _levelLoadComplete)
                {
                    GameObject curCutScene = _currScenes[0];
                    _currScenes.RemoveAt(0);
                    curCutScene.SetActive(false);
                }
            }
            else
            {
                _levelManager.freezePlayer = false;
                this.gameObject.SetActive(false);
            }
        }
    }

    void OnEnable()
    {
        _levelLoadComplete = true;
    }

    public void TriggerEndCutScene()
    {
        foreach (GameObject i in _endSceneStorage)
        {
            i.gameObject.SetActive(true);
            print("activated");
        }
        endSceneActive = true;
        this.gameObject.SetActive(true);
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
