using UnityEngine;

public class Interactable : MonoBehaviour
{
    // For highlighting the selected block.
    protected Material Material;
    public Color originalColour;
    public Color paintedColour;

    private Material[] _allMaterials;
    private Material[] _originalMaterials;
    protected bool IsMouseOver;
    protected void Start()
    {
        Material = GetComponentInChildren<Renderer>().material;
        originalColour = Material.color;
        paintedColour = Material.color;
        
        // Material = GetComponentInChildren<Renderer>().material;

    }

    public void UndoHighlight()
    {
        if (Material)
        {
            Material.color = paintedColour;
        }
    }

    public void HighlightForHoverover()
    {
        if (Material)
        {
            Material.color = new Color(
            tintColour(Material.color.r, GameConstants.HOVEROVER_R, 0.2f),
            tintColour(Material.color.g, GameConstants.HOVEROVER_G, 0.2f),
            tintColour(Material.color.b, GameConstants.HOVEROVER_B, 0.2f));
        }
    }

    public void HighlightForPaintSelectionUI()
    {
        if (Material)
        {
            Material.color = new Color(
                tintColour(Material.color.r, GameConstants.SELECTION_R, 0.2f),
                tintColour(Material.color.g, GameConstants.SELECTION_G, 0.2f),
                tintColour(Material.color.b, GameConstants.SELECTION_B, 0.2f));
        }
    }
    
    public void HighlightForPaintSelectionUIUninteractable()
    {
        if (Material)
        {
            Material.color = new Color(
                tintColour(Material.color.r, GameConstants.UNINTERACTABLE_SELECTION_R, 0.5f),
                tintColour(Material.color.g, GameConstants.UNINTERACTABLE_SELECTION_G, 0.5f),
                tintColour(Material.color.b, GameConstants.UNINTERACTABLE_SELECTION_B, 0.5f));
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