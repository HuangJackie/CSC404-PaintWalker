using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Menu
{
    private OptionsMenu optionsMenu;
    private LevelSelectMenu levelSelectMenu;

    protected override void Start()
    {
        base.Start();
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
