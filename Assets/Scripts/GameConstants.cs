using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConstants : MonoBehaviour
{
    // Identifiers for conditionals
    public const int RED_PAINT = 0;
    public const int GREEN_PAINT = 1;
    public const int YELLOW_PAINT = 2;
    public const int BLUE_PAINT = 3;

    // Paint colors
    public static readonly Color32 red = new Color32(241, 95, 62, 255);
    public static readonly Color32 green = new Color32(166, 191, 75, 255);
    public static readonly Color32 yellow = new Color32(242, 191, 61, 255);
    public static readonly Color32 blue = new Color32(140, 210, 205, 255);

    // Highlight interactable object colours
    public const float HOVEROVER_R = 0.98f;
    public const float HOVEROVER_G = 1f;
    public const float HOVEROVER_B = 0.45f;
    
    public const float SELECTION_R = 0.87f;
    public const float SELECTION_G = 0.45f;
    public const float SELECTION_B = 1f;
    // public static readonly Color32 TOOLTIP_HIGHLIGHT_COLOUR = new Color(0.98f, 1f, 0.45f, 0.03f);
    // public static readonly Color32 SELECTION_HIGHLIGHT_COLOUR = new Color(0.87f, 0.45f, 1f, 0.03f);
}
