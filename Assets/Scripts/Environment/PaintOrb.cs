using System.Collections;
using System.Collections.Generic;
using System;
using DefaultNamespace;
using UnityEngine;
using static GameConstants;

public class PaintOrb : Interactable, TooltipObject
{
    [Header("Manager")]
    public LevelManager manager;

    [Header("Paint Information")]
    public Paints paint;
    public int paintReplenished;

    private ParticleSystem.MainModule particleSettings;
    private Light lightSettings;
    private Renderer[] meshRenderers;
    private UpdateUI _updateUI;

    private new void Start()
    {
        base.Start();
        particleSettings = GetComponentInChildren<ParticleSystem>().main;
        lightSettings = GetComponentInChildren<Light>();
        meshRenderers = GetComponentsInChildren<Renderer>();
        _updateUI = FindObjectOfType<UpdateUI>();

        Color newColor;
        switch (paint)
        {
            case Paints.Yellow:
                newColor = GameConstants.Yellow;
                break;
            case Paints.Red:
                newColor = GameConstants.Red;
                break;
            case Paints.Green:
                newColor = GameConstants.Green;
                break;
            case Paints.Blue:
                newColor = GameConstants.Blue;
                break;
            default:
                newColor = GameConstants.Yellow;
                break;
        }

        particleSettings.startColor = newColor;
        lightSettings.color = newColor;
        foreach (Renderer mesh in meshRenderers)
        {
            mesh.material.color = newColor;
            mesh.material.SetFloat("_Glossiness", 0.4f);
        }
        ObjectStorage.paintOrbStorage.Add(this.gameObject);
        ReinitializeMaterialColours();
    }

    
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            manager.IncreasePaint(paint, paintReplenished);
            gameObject.SetActive(false);
        }
    }

    private new void OnMouseEnter()
    {
        base.OnMouseEnter();
        if (this)
        {
            OnDisplayTooltip();
        }
    }

    private new void OnMouseExit()
    {
        base.OnMouseExit();
        if (this)
        {
            OnExitTooltip();
        }
    }

    public void OnDisplayTooltip()
    {
        _updateUI.SetInfoText("Replenishes: " + paintReplenished +
                              " " + Enum.GetName(typeof(Paints), paint));
        HighlightForHoverover();

    }

    public void OnExitTooltip()
    {
        _updateUI.WipeInfoText();
        UndoHighlight();
    }
}