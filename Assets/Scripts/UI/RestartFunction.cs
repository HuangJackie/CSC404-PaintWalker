using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class RestartFunction : MonoBehaviour
{
    public static void Restart()
    {
        SceneLoader.RestartLevel();
        Time.timeScale = 1f;
    }
}
