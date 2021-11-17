using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameConstants;

public class SceneLoader : MonoBehaviour
{
    public static void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public static void LoadLevel(Levels level)
    {
        SceneManager.LoadScene(Enum.GetName(typeof(Levels), level));
    }

    public static void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
