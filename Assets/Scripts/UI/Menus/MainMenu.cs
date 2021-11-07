using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Menu
{
    private OptionsMenu optionsMenu;

    protected override void Start()
    {
        base.Start();
        optionsMenu = FindObjectOfType<OptionsMenu>();
    }

    public void LoadOptionsPanel()
    {
        optionsMenu.Load(gameObject);
    }

    public static void ExitGame()
    {
        Application.Quit();
    }
}
