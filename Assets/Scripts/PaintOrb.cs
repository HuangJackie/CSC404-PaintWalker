using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class PaintOrb : Interactable, TooltipObject
{
    public LevelManager manager;
    public string color;
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
        switch (color)
        {
            case "Yellow":
                newColor = GameConstants.yellow;
                break;
            case "Blue":
                newColor = GameConstants.blue;
                break;
            case "Green":
                newColor = GameConstants.green;
                break;
            case "Red":
                newColor = GameConstants.red;
                break;
            default:
                newColor = GameConstants.yellow;
                break;
        }

        particleSettings.startColor = newColor;
        lightSettings.color = newColor;
        foreach (Renderer mesh in meshRenderers)
        {
            mesh.material.color = newColor;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            manager.IncreasePaint(color, paintReplenished);
            gameObject.SetActive(false);
            MoveRedo lastCommand = manager.redoCommandHandler.LatestCommand() as MoveRedo;
            if (lastCommand)
            {
                lastCommand.InjectPaintPickup(this.gameObject);
            }
        }
    }

    private new void OnMouseOver()
    {
        base.OnMouseOver();
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
        _updateUI.SetInfoText("Replenishes: " + paintReplenished + " " + color);
        HighlightForHoverover();

    }

    public void OnExitTooltip()
    {
        _updateUI.WipeInfoText();
        UndoHighlight();
    }
}