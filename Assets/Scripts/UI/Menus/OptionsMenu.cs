using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : Menu
{
    private GameObject returningMenu;  // Panel to activate when exiting OptionsMenu
    private GameObject optionsMenuRenderer;

    protected override void Start()
    {
        base.Start();
        // First child should be the options renderer
        optionsMenuRenderer = transform.GetChild(0).gameObject;
    }

    // Load the OptionsMenu associated with this script
    public void Load(GameObject returningMenu)
    {
        this.returningMenu = returningMenu;
        this.returningMenu.SetActive(false);
        optionsMenuRenderer.SetActive(true);
    }

    // Exit this OptionsMenu, returning to the previous menu returningPanel
    public void Exit()
    {
        optionsMenuRenderer.SetActive(false);
        returningMenu.SetActive(true);
        returningMenu = null;
    }
}
