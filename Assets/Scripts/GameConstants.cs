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
    public const float HOVEROVER_R = 1f;
    public const float HOVEROVER_G = 1f;
    public const float HOVEROVER_B = 0f;
    
    public const float SELECTION_R = 0.1f;
    public const float SELECTION_G = 0.1f;
    public const float SELECTION_B = 0.1f;
    // public static readonly Color32 TOOLTIP_HIGHLIGHT_COLOUR = new Color(1f, 0.99f, 0f);
    // public static readonly Color32 SELECTION_HIGHLIGHT_COLOUR = new Color(0.26f, 0.26f, 0.26f);

    /*
     * Note this is for the switch paint UI but they're not pointing in the right direction.
     * Everything seems to be rotated, but still works. Just need to rename later.
     */
    public const string NorthwestQuadrant = "NW";
    public const string NortheastQuadrant = "NE";
    public const string SouthwestQuadrant = "SW";
    public const string SoutheastQuadrant = "SE";
}
