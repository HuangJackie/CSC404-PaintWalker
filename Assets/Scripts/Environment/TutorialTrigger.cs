using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    private TutorialPromptsManager tutorialPrompts;
    public Tutorial tutorial;

    void Start()
    {
        tutorialPrompts = FindObjectOfType<TutorialPromptsManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            tutorialPrompts.DisplayPrompt(tutorial);
            gameObject.SetActive(false);  // Disable this trigger
        }
    }
}
