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

    public static void LoadNextLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "Tutorial1")
        {
            LoadLevel(Levels.Tutorial15);
        } else if (scene.name == "Tutorial15")
        {
            LoadLevel(Levels.Tutorial2);
        } else if (scene.name == "Tutorial2")
        {
            LoadLevel(Levels.Level1);
        }  else if (scene.name == "Level1")
        {
            LoadLevel(Levels.Level2);
        } else if (scene.name == "Level2")
        {
            LoadLevel(Levels.Level3);
        } else
        {
            LoadMainMenu();
        }
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
