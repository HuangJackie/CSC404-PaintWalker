using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public abstract class SpecialCreature : Interactable, TooltipObject, Paintable
{
    public string paintColour1;
    public string paintColour2;
    public int paintQuantity1;
    public int paintQuantity2;

    private UpdateUI _updateUI;
    public bool isPainted;
    

    protected new void Start()
    {
        base.Start();
        _updateUI = FindObjectOfType<UpdateUI>();
        ObjectStorage.specialCreatureStorage.Add(this);
    }

    protected void OnMouseDown()
    {
        Paint(true);
        _updateUI.WipeInfoText();
        UndoHighlight();
    }

    new void OnMouseEnter()
    {
        base.OnMouseEnter();
        IsMouseOver = true;
        OnDisplayTooltip();
    }

    new void OnMouseExit()
    {
        base.OnMouseExit();
        IsMouseOver = false;
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
            HighlightForHoverover();
        }
    }

    public void OnExitTooltip()
    {
        if (!isPainted)
        {
            _updateUI.WipeInfoText();
            UndoHighlight();
        }
    }
    public abstract bool Paint(bool paintWithBrush);
}