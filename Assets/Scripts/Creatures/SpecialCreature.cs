using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using static GameConstants;

public abstract class SpecialCreature : Interactable, TooltipObject, Paintable
{
    // Paints required for activation
    public Paints paintType1;
    public Paints paintType2;
    public int paintQuantity1;
    public int paintQuantity2;

    // Status
    public bool isPainted;

    // Models for frozen/unfrozen states
    public GameObject frozen_model;
    public GameObject coloured_model;

    // Other required objects
    private UpdateUI _updateUI;

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
            _updateUI.SetInfoText("Needs: " + paintQuantity1 + " " +
                                  Enum.GetName(typeof(Paints), paintType1) +
                                  " " + paintQuantity2 + " " +
                                  Enum.GetName(typeof(Paints), paintType2));
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
    
    // Not implemented for special creatures currently.
    public bool IsPaintable()
    {
        return true;
    }
}