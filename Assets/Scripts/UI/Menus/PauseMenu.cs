using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DefaultNamespace;
using static GameConstants;

public class PauseMenu : SecondaryMenu
{
    private LevelManager levelManager;
    private OptionsMenu optionsMenu;
    private ControlsMenu controlsMenu;

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
            if (!controllerUtil.GetIsMenuOpen())  // If in-game
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
        Close();
        base.transitionAnimation.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneLoader.LoadMainMenu();
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
        Close();
        levelManager.RestartAtLastCheckpoint();
    }

    public void RestartLevel()
    {
        Close();
        SceneLoader.RestartLevel();
    }
}
