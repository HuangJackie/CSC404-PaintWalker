using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    private TutorialPromptsManager tutorialPrompts;
    private CutSceneManager cutsceneManager;
    public Tutorial tutorial;

    void Start()
    {
        tutorialPrompts = FindObjectOfType<TutorialPromptsManager>();
        cutsceneManager = FindObjectOfType<CutSceneManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (cutsceneManager && cutsceneManager.ShowingCutscenes()) return;
        if (other.tag == "Player")
        {
            tutorialPrompts.DisplayPrompt(tutorial);
            gameObject.SetActive(false);  // Disable this trigger
        }
    }
}
