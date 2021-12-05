using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DefaultNamespace;

public class TutorialPromptsManager : MonoBehaviour
{
    private ControllerUtil controllerUtil;
    private Canvas renderingCanvas;

    private Canvas currPromptCanvas;
    [SerializeField] private Canvas imagePromptCanvas;
    [SerializeField] private Canvas sentencePromptCanvas;

    private Tutorial currTutorial;
    private int currTutorialPageDisplaying;

    private void Awake()
    {
        controllerUtil = FindObjectOfType<ControllerUtil>();
        renderingCanvas = transform.GetChild(0).GetComponent<Canvas>();

        renderingCanvas.enabled = false;
        imagePromptCanvas.enabled = false;
        sentencePromptCanvas.enabled = false;
        currPromptCanvas = null;

        currTutorial = null;
        currTutorialPageDisplaying = 0;
    }

    private void Update()
    {
        if (controllerUtil.GetTutorialPromptContButton())
        {
            ContinuePrompt();
        }
    }

    public void DisplayPrompt(Tutorial tutorial)
    {
        // Check if tutorial is valid
        if (tutorial.sentences.Length == 0)
        {
            return;
        }

        // If not null, we are replacing a tutorial on screen
        if (currTutorial != null)
        {
            ClearCurrPrompt();
            ClearCurrTutorial();
        }

        // Set the currently displaying tutorial
        currTutorial = tutorial;

        // Choose which prompt type to use (with image or without)
        // and display the first page of tutorial
        EnableRendering();
        if (currTutorial.images.Length == 0)
        {
            currPromptCanvas = sentencePromptCanvas;
            SetCurrTutPageAssets(currTutorial.sentences[currTutorialPageDisplaying], null);
        }
        else
        {
            currPromptCanvas = imagePromptCanvas;
            SetCurrTutPageAssets(currTutorial.sentences[currTutorialPageDisplaying],
                                 currTutorial.images[currTutorialPageDisplaying]);
        }

        currPromptCanvas.enabled = true;
    }

    private void ContinuePrompt()
    {
        // Are all pages finished displaying?
        currTutorialPageDisplaying++;
        if (currTutorialPageDisplaying >= currTutorial.sentences.Length)
        {
            DisableRendering();
            return;
        }

        // Display next page based onf if there is
        // an image to display or not
        ClearCurrPrompt();
        if (currTutorialPageDisplaying < currTutorial.images.Length)
        {
            currPromptCanvas = imagePromptCanvas;
            SetCurrTutPageAssets(currTutorial.sentences[currTutorialPageDisplaying],
                                 currTutorial.images[currTutorialPageDisplaying]);
        }
        else
        {
            currPromptCanvas = sentencePromptCanvas;
            SetCurrTutPageAssets(currTutorial.sentences[currTutorialPageDisplaying], null);
        }

        currPromptCanvas.enabled = true;
    }

    // Sets and updates tutorial assets according to
    // currTutorialPageDisplaying
    private void SetCurrTutPageAssets(string sentence, Sprite image)
    {
        // Check if valid arguments. There must be at
        // least a sentence to update.
        if (sentence == null || sentence.Length == 0)
        {
            return;
        }

        // Change prompt text
        TextMeshProUGUI promptText = currPromptCanvas
                                     .GetComponentInChildren<TextMeshProUGUI>();
        if (promptText) promptText.text = sentence;

        // Change prompt image if needed
        if (image != null)
        {
            Image promptImage = currPromptCanvas.GetComponentInChildren<Image>();
            if (promptImage) promptImage.sprite = image;
        }
    }

    private void EnableRendering()
    {
        controllerUtil.OpenTutorialPrompt();
        renderingCanvas.enabled = true;
    }

    private void DisableRendering()
    {
        controllerUtil.CloseTutorialPrompt();
        renderingCanvas.enabled = false;
        ClearCurrPrompt();
        ClearCurrTutorial();
    }

    private void ClearCurrPrompt()
    {
        if (currPromptCanvas == null) return;
        currPromptCanvas.enabled = false;

        Image promptImage = currPromptCanvas.GetComponentInChildren<Image>();
        TextMeshProUGUI promptText = currPromptCanvas
                                     .GetComponentInChildren<TextMeshProUGUI>();
        if (promptImage) promptImage.sprite = null;
        if (promptText) promptText.text = "";

        currPromptCanvas = null;
    }

    private void ClearCurrTutorial()
    {
        currTutorial = null;
        currTutorialPageDisplaying = 0;
    }
}
