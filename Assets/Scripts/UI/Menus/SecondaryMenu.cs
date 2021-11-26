using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefaultNamespace;

public class SecondaryMenu : Menu
{
    // returningMenu - Panel to activate when exiting this SecondaryMenu
    protected GameObject returningMenu;
    protected GameObject menuRenderer;

    protected override void Start()
    {   
        // First child should be the renderer for this SecondaryMenu
        menuRenderer = transform.GetChild(0).gameObject;
        menuRenderer.SetActive(false);
        returningMenu = null;

        // Do same initialization as parenting class Menu
        controllerUtil = FindObjectOfType<ControllerUtil>();
        // Populate buttons from children of menuRenderer
        PopulateButtons(menuRenderer.transform);
    }

    protected override void Update()
    {
        if (menuRenderer.activeSelf)
        {
            base.Update();
            if (ControllerUtil.GetCancelButtonPressed())
            {
                Close();
            }
        }
    }

    // Load the SecondaryMenu associated with this script
    public void LoadSelf(GameObject returningMenu)
    {
        this.returningMenu = returningMenu;
        this.returningMenu.SetActive(false);
        menuRenderer.SetActive(true);
    }

    // Exit this SecondaryMenu, returning to returningMenu
    public void Close()
    {
        menuRenderer.SetActive(false);
        if (returningMenu != null)
        {
            returningMenu.SetActive(true);
            returningMenu = null;
        }
    }
}
