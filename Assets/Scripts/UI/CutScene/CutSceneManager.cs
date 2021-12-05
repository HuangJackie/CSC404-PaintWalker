using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DefaultNamespace;

public class CutSceneManager : MonoBehaviour
{
    private List<GameObject> _cutSceneStorage;
    private LevelManager _levelManager;
    private CutSceneDontDeleteManager _cutSceneDontDestroyManager;
    private ControllerUtil _controllerUtil;

    private static GameObject instance;
    private bool _levelLoadComplete;

    void Start()
    {
        instance = gameObject;
        _levelManager = FindObjectOfType<LevelManager>();
        _cutSceneDontDestroyManager = FindObjectOfType<CutSceneDontDeleteManager>();
        _controllerUtil = FindObjectOfType<ControllerUtil>();

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
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Update()
    {
        if (_cutSceneStorage.Count > 0)
        {
            _levelManager.freezePlayer = true;
            if ((Input.GetMouseButtonDown(0)               ||
                 _controllerUtil.GetConfirmButtonPressed() ||
                 _controllerUtil.GetCancelButtonPressed()) &&
                _levelLoadComplete)
            {
                GameObject curCutScene = _cutSceneStorage[0];
                _cutSceneStorage.RemoveAt(0);
                curCutScene.SetActive(false);
            }
        }
        else
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
