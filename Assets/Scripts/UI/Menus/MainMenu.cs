using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DefaultNamespace;

public class MainMenu : Menu
{
    protected GameObject menuRenderer;
    [SerializeField] private Image gameLogo;

    private OptionsMenu optionsMenu;
    private LevelSelectMenu levelSelectMenu;

    protected override void Start()
    {
        // First child should be the renderer for MainMenu
        menuRenderer = transform.GetChild(0).gameObject;
        menuRenderer.SetActive(true);

        // Do same initialization as parenting class Menu
        controllerUtil = FindObjectOfType<ControllerUtil>();
        // Populate buttons from children of menuRenderer
        PopulateButtons(menuRenderer.transform);

        // Prepare SecondaryMenus
        optionsMenu = FindObjectOfType<OptionsMenu>();
        levelSelectMenu = FindObjectOfType<LevelSelectMenu>();
    }

    public void LoadOptionsMenu()
    {
        optionsMenu.LoadSelf(gameObject);
    }

    public void LoadLevelSelectMenu()
    {
        levelSelectMenu.LoadSelf(gameObject);
    }

    public static void ExitGame()
    {
        Application.Quit();
    }
}
