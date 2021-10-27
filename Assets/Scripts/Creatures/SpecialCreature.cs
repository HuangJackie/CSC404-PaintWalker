using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class SpecialCreature : MonoBehaviour, TooltipObject
{
    public string paintColour1;
    public string paintColour2;
    public int paintQuantity1;
    public int paintQuantity2;

    private UpdateUI _updateUI;
    // private bool _isMouseClicked;

    protected bool IsMouseOver;

    protected Material Material;
    protected Color OriginalColour;
    public bool isPainted;

    protected void Start()
    {
        _updateUI = FindObjectOfType<UpdateUI>();
        Material = GetComponentInChildren<Renderer>().material;
        OriginalColour = Material.color;
    }

    void OnMouseOver()
    {
        print("mouse over");

        OnDisplayTooltip();
    }

    void OnMouseExit()
    {
        if (isPainted)
        {
            return;
        }

        OnExitTooltip();
    }

    public void OnDisplayTooltip()
    {
        if (!isPainted)
        {
            _updateUI.SetInfoText("Needs: " + paintQuantity1 + " " + paintColour1 +
                                  " " + paintQuantity2 + " " + paintColour2);
            Material.color = new Color(0.98f, 1f, 0.45f);
        }
    }

    public void OnExitTooltip()
    {
        if (!isPainted)
        {
            _updateUI.SetInfoText("");
            Material.color = OriginalColour;
        }
    }
}