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
    private GameObject[] _gameObjects;
    private int _selectedMenuOption;
    private const int TotalNumberOfMenuOptions = 2;

    public GameObject resume;
    public GameObject menu;

    private void Start()
    {
        _controllerUtil = FindObjectOfType<ControllerUtil>();
        _menuOptions = new Button[2];
        _menuOptions[0] = resume.GetComponentInChildren<Button>();
        _menuOptions[1] = menu.GetComponentInChildren<Button>();
        _selectedMenuOption = 1;
    }

    
    // Update is called once per frame

    void Update()
    {
        if (_controllerUtil.GetMenuButtonPressed())
        {
            if (gameIsPaused)
            {
                print("resumed");
                Resume();
            }
            else
            {
                print("paused");

                _menuOptions[_selectedMenuOption].OnPointerEnter(null);
                Pause();
            }
        }
        if (controlUIActive && Input.anyKey)
        {
            HideControl();
        }

        if (gameIsPaused)
        {
            if (_controllerUtil.GetConfirmButtonPressed())
            {
                print("clicked" + _selectedMenuOption);
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
        print("set to false in resume");
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
        SceneManager.LoadScene("MainMenuScene");
    }
}