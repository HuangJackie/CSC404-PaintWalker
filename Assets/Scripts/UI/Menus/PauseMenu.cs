using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameConstants;

public class PauseMenu : SecondaryMenu
{
    private LevelManager levelManager;
    private OptionsMenu optionsMenu;
    private ControlsMenu controlsMenu;
    private RestartDontDeleteManager restartDontDeleteManager;
    
    private Image background;

    private void Awake()
    {
        background = GetComponent<Image>();
        background.enabled = false;
    }

    protected override void Start()
    {
        base.Start();
        levelManager = FindObjectOfType<LevelManager>();
        optionsMenu = FindObjectOfType<OptionsMenu>();
        controlsMenu = FindObjectOfType<ControlsMenu>();
        restartDontDeleteManager = FindObjectOfType<RestartDontDeleteManager>();
    }

    protected override void Update()
    {
        if (menuRenderer.activeSelf)
        {
            base.UpdateBase();  // Run Update() in Menu parent class
            if (controllerUtil.GetCancelButtonPressed())
            {
                Close();
            }
        }

        if (controllerUtil.GetMenuButtonPressed())
        {
            if (!controllerUtil.GetMenuOpen())  // If in-game
            {
                LoadSelf();
            }
            else if (menuRenderer.activeSelf)  // If in the pause menu
            {
                Close();
            }
            else  // If in one of the pause menu's submenus
            {
                CloseAllSubmenus();
                Close();
            }
        }
    }

    public void SetBackgroundColor(Paints paintColor)
    {
        switch (paintColor)
        {
            case Paints.Yellow:
                background.color = Yellow;
                break;
            case Paints.Red:
                background.color = Red;
                break;
            case Paints.Green:
                background.color = Green;
                break;
            case Paints.Blue:
                background.color = Blue;
                break;
        }
    }

    public void LoadSelf()
    {
        controllerUtil.OpenMenu();
        background.enabled = true;
        SetBackgroundColor(levelManager.GetCurrentlySelectedPaint());
        menuRenderer.SetActive(true);
    }

    // Overwrite SecondaryMenu.LoadSelf(GameObject) to use LoadSelf()
    public new void LoadSelf(GameObject returningMenu)
    {
        LoadSelf();
    }

    /* // Close down the pause menu */
    public new void Close()
    {
        controllerUtil.CloseMenu();
        background.enabled = false;
        menuRenderer.SetActive(false);
    }

    public void CloseAllSubmenus()
    {
        optionsMenu.Close();
        controlsMenu.Close();
    }

    // Menu Button functionality
    // ----------------------------

    public void LoadMainMenu()
    {
        restartDontDeleteManager = FindObjectOfType<RestartDontDeleteManager>();
        restartDontDeleteManager.isRestarting = false;
        Close();
        base.transitionAnimation.SetTrigger("FadeOut");
    }

    public void FadeOut()
    {
        base.transitionAnimation.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneLoader.LoadNextLevel();
    }

    public void LoadOptionsMenu()
    {
        optionsMenu.LoadSelf(menuRenderer.gameObject);
    }

    public void LoadControlsMenu()
    {
        controlsMenu.LoadSelf(menuRenderer.gameObject);
    }

    public void LoadCheckpoint()
    {
        restartDontDeleteManager = FindObjectOfType<RestartDontDeleteManager>();
        restartDontDeleteManager.isRestarting = true;
        Close();
        levelManager.RestartAtLastCheckpoint();
    }

    public void RestartLevel()
    {
        restartDontDeleteManager = FindObjectOfType<RestartDontDeleteManager>();
        restartDontDeleteManager.isRestarting = true;
        Close();
        SceneLoader.RestartLevel();
    }
}
