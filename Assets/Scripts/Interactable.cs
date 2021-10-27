using UnityEngine;

public class Interactable : MonoBehaviour
{
    // For highlighting the selected block.
    protected Material Material;
    public Color originalColour;

    protected bool IsMouseOver;
    protected void Start()
    {
        Material = GetComponentInChildren<Renderer>().material;
        originalColour = Material.color;
    }

    public void UndoHighlight()
    {
        Material.color = originalColour;
    }

    public void HighlightForHoverover()
    {
        Material.color = new Color(
            tintColour(Material.color.r, GameConstants.HOVEROVER_R),
            tintColour(Material.color.g, GameConstants.HOVEROVER_G),
            tintColour(Material.color.b, GameConstants.HOVEROVER_B));
    }

    public void HighlightForPaintSelectionUI()
    {
        Material.color = new Color(
            tintColour(Material.color.r, GameConstants.SELECTION_R) + 0.1f,
            tintColour(Material.color.g, GameConstants.SELECTION_G),
            tintColour(Material.color.b, GameConstants.SELECTION_B));
    }

    private float tintColour(float colourToTint, float tint)
    {
        return colourToTint + (tint - colourToTint) * 0.2f;
    }

    protected void OnMouseOver()
    {
        IsMouseOver = true;
    }
    
    protected void OnMouseExit()
    {
        IsMouseOver = false;
    }
}