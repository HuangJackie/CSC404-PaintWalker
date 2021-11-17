using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    // For highlighting the selected block.
    protected Material Material;
    public Color originalColour;
    public Color paintedColour;

    private List<Material> _allMaterials;
    public List<Color> _allColours;
    private bool _isTextureDark;

    protected bool IsMouseOver;

    protected void Start()
    {
        Material = GetComponentInChildren<Renderer>().material;
        originalColour = Material.color;
        paintedColour = Material.color;
        ReinitializeMaterialColours();
    }

    public void ReinitializeMaterialColours()
    {
        if (TryGetComponent(out TutorialToolTips _))
        {
            _isTextureDark = true;
        }
        
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        _allMaterials = new List<Material>();
        _allColours = new List<Color>();
        foreach (var renderer in renderers)
        {

            Material rendererMaterial = renderer.material;
            if (rendererMaterial.HasProperty("_Color"))
            {
                _allMaterials.Add(renderer.material);
                _allColours.Add(renderer.material.color);
            }
        }
    }

    public void UndoHighlight()
    {
        if (Material)
        {
            Material.color = paintedColour;
        }

        if (_allMaterials != null)
        {
            for (int i = 0; i < _allMaterials.Count; i++)
            {
                _allMaterials[i].color = _allColours[i];
            }
        }
    }

    public void HighlightForHoverover()
    {
        float amountToTint = 0.2f;
        Color color = new Color(GameConstants.SELECTION_R, GameConstants.SELECTION_G, GameConstants.SELECTION_B);
        if (_isTextureDark)
        {
            color = new Color(GameConstants.SELECTION_LIGHT_R, GameConstants.SELECTION_LIGHT_G, GameConstants.SELECTION_LIGHT_B);
            amountToTint = 1f;
        }
        if (Material)
        {
            Material.color = new Color(
                tintColour(Material.color.r, color.r, amountToTint),
                tintColour(Material.color.g, color.g, amountToTint),
                tintColour(Material.color.b, color.b, amountToTint));
        }
        
        if (_allMaterials != null)
        {
            foreach (var childMaterial in _allMaterials)
            {
                Color materialColour = childMaterial.color;
                childMaterial.color = new Color(
                    tintColour(materialColour.r, color.r, amountToTint),
                    tintColour(materialColour.g, color.g, amountToTint),
                    tintColour(materialColour.b, color.b, amountToTint));
            }
        }
    }

    public void HighlightForPaintSelectionUI()
    {
        float amountToTint = 0.2f;
        Color color = new Color(GameConstants.SELECTION_R, GameConstants.SELECTION_G, GameConstants.SELECTION_B);
        if (_isTextureDark)
        {
            color = new Color(GameConstants.SELECTION_LIGHT_R, GameConstants.SELECTION_LIGHT_G, GameConstants.SELECTION_LIGHT_B);
            amountToTint = 1f;
        }
        
        if (Material)
        {
            Material.color = new Color(
                tintColour(Material.color.r, color.r, amountToTint),
                tintColour(Material.color.g, color.g, amountToTint),
                tintColour(Material.color.b, color.b, amountToTint));
        }

        if (_allMaterials != null)
        {
            foreach (var childMaterial in _allMaterials)
            {
                Color materialColour = childMaterial.color;
                childMaterial.color = new Color(
                    tintColour(materialColour.r, color.r, amountToTint),
                    tintColour(materialColour.g, color.g, amountToTint),
                    tintColour(materialColour.b, color.b, amountToTint));
            }
        }
    }

    public void HighlightForPaintSelectionUIUninteractable()
    {
        if (Material)
        {
            Material.color = new Color(
                tintColour(Material.color.r, GameConstants.UNINTERACTABLE_SELECTION_R, 0.3f),
                tintColour(Material.color.g, GameConstants.UNINTERACTABLE_SELECTION_G, 0.3f),
                tintColour(Material.color.b, GameConstants.UNINTERACTABLE_SELECTION_B, 0.3f));
        }
        
        if (_allMaterials != null)
        {
            foreach (var childMaterial in _allMaterials)
            {
                Color materialColour = childMaterial.color;
                childMaterial.color = new Color(
                    tintColour(materialColour.r, GameConstants.UNINTERACTABLE_SELECTION_R, 0.3f),
                    tintColour(materialColour.g, GameConstants.UNINTERACTABLE_SELECTION_G, 0.3f),
                    tintColour(materialColour.b, GameConstants.UNINTERACTABLE_SELECTION_B, 0.3f));
            }
        }
    }

    private float tintColour(float colourToTint, float tint, float amountToTint)
    {
        return colourToTint + (tint - colourToTint) * amountToTint;
    }

    protected void OnMouseEnter()
    {
        IsMouseOver = true;
    }

    protected void OnMouseExit()
    {
        IsMouseOver = false;
    }
}