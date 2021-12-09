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
    private PauseMenu _pauseMenu;
    private bool fadingIn;

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
        _pauseMenu = FindObjectOfType<PauseMenu>();

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
        
        if (_currScenes != null && !fadingIn) {
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
                    _controllerUtil.CloseMenu();
                }
            }
            else if (!fadingIn)
            {
                _pauseMenu.FadeIn();
                fadingIn = true;
                
            }
        }

        if (fadingIn)
        {
            if (_pauseMenu.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("LevelFadeTransition") &&
            _pauseMenu.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1 &&
            !_pauseMenu.GetComponent<Animator>().IsInTransition(0))
            {
                _levelManager.freezePlayer = false;
                this.gameObject.SetActive(false);
                fadingIn = false;
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
