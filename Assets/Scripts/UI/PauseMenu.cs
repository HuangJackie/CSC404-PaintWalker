using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;
    private bool controlUIActive = false;

    public GameObject pauseMenuUI;
    public GameObject controlMenuUI;
    // Update is called once per frame

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
        if (controlUIActive && Input.anyKey)
        {
            HideControl();
        }
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        controlMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
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
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

}
