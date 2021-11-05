using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    // Start is called before the first frame update
    public void StartGame()
    {
        SceneManager.LoadScene("AlphaScenev2");
    }

    public void StartTutorial()
    {
        SceneManager.LoadScene("TutorialColors");
    }
}
