using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;
    private bool controlUIActive = false;

    public GameObject pauseMenuUI;
    public GameObject controlMenuUI;

    private ControllerUtil _controllerUtil;
    private Button[] _menuOptions;
    private int _selectedMenuOption;
    private const int TotalNumberOfMenuOptions = 5;

    public GameObject resume;
    public GameObject menu;
    public GameObject control;
    public GameObject loadCheckpoint;
    public GameObject restart;

    private LevelManager _levelManager;

    private void Start()
    {
        _levelManager = FindObjectOfType<LevelManager>();
        _controllerUtil = FindObjectOfType<ControllerUtil>();
        _menuOptions = new Button[TotalNumberOfMenuOptions];

        _menuOptions[0] = resume.GetComponentInChildren<Button>();
        _menuOptions[1] = control.GetComponentInChildren<Button>();
        _menuOptions[2] = menu.GetComponentInChildren<Button>();
        _menuOptions[3] = loadCheckpoint.GetComponentInChildren<Button>();
        _menuOptions[4] = restart.GetComponentInChildren<Button>();
        _selectedMenuOption = 0;
    }

    
    // Update is called once per frame

    void Update()
    {
        if (_controllerUtil.GetMenuButtonPressed())
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                _menuOptions[_selectedMenuOption].OnPointerEnter(null);
                Pause();
            }
        }
        if (controlUIActive && (_controllerUtil.GetConfirmButtonPressed() || Input.GetKeyDown(KeyCode.RightShift)))
        {
            HideControl();
        }

        if (gameIsPaused)
        {
            if (_controllerUtil.GetConfirmButtonPressed())
            {
                _menuOptions[_selectedMenuOption].onClick.Invoke();
                _selectedMenuOption = 0;
                if (_selectedMenuOption == 0)
                {
                    _controllerUtil.CloseMenu();
                }
            }
            
            if (_controllerUtil.GetGameMenuSelectAxis(out int select))
            {
                _menuOptions[_selectedMenuOption].OnPointerExit(null);
                if (select > 0)
                {
                    IncrementMenuOption();
                }
                else
                {
                    DecrementMenuOption();
                }

                _menuOptions[_selectedMenuOption].OnPointerEnter(null);
            }
        }

        ListenForLoadCheckpoint();
    }

    private void ListenForLoadCheckpoint()
    {
        if (_controllerUtil.loadCheckpointPressed())
        {
            _levelManager.RestartAtLastCheckpoint();
        }
    }

    private void DecrementMenuOption()
    {
        _selectedMenuOption--;
        if (_selectedMenuOption == -1)
        {
            _selectedMenuOption = TotalNumberOfMenuOptions - 1;
        }
    }

    private void IncrementMenuOption()
    {
        _selectedMenuOption++;
        if (_selectedMenuOption == TotalNumberOfMenuOptions)
        {
            _selectedMenuOption = 0;
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        controlMenuUI.SetActive(false);
        //Time.timeScale = 1f;
        gameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        // Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void ShowControl()
    {
        pauseMenuUI.SetActive(false);
        controlMenuUI.SetActive(true);
        controlUIActive = true;
    }

    void HideControl()
    {
        pauseMenuUI.SetActive(true);
        controlMenuUI.SetActive(false);
        controlUIActive = false;
    }

    public void LoadMenu()
    {
        // Time.timeScale = 1f;
        SceneLoader.LoadMainMenu();
    }

    public void LoadCheckpoint()
    {
        _levelManager.RestartAtLastCheckpoint();
        Resume();
    }
    
    public void Restart()
    {
        RestartFunction.Restart();
        Resume();
    }
}