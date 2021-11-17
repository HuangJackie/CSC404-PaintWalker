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

    // Identifiers for paint colours
    public const string RedColour = "Red";
    public const string GreenColour = "Green";
    public const string YellowColour = "Yellow";
    public const string BlueColour = "Blue";
    
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
    public const float SELECTION_LIGHT_R = 1f;
    public const float SELECTION_LIGHT_G = 1f;
    public const float SELECTION_LIGHT_B = 1f;
    
    public const float UNINTERACTABLE_SELECTION_R = 1f;
    public const float UNINTERACTABLE_SELECTION_G = 0.2f;
    public const float UNINTERACTABLE_SELECTION_B = 0f;
    // public static readonly Color32 SELECTION_HIGHLIGHT_COLOUR = new Color(0.26f, 0.26f, 0.26f);

    /*
     * Note this is for the switch paint UI but they're not pointing in the right direction.
     * Everything seems to be rotated, but still works. Just need to rename later.
     */
    public const string NorthwestQuadrant = "NW";
    public const string NortheastQuadrant = "NE";
    public const string SouthwestQuadrant = "SW";
    public const string SoutheastQuadrant = "SE";

    /* The Levels enum denotes the various levels available to the Player.
     * Levels should be named after their respective Scene names.
     */
    public enum Levels {
        AlphaScenev2,
        TutorialColors,

        Level1,
        Tutorial2,
        Tutorial15,
        Tutorial1
    }

    /*
     * Used to determine which way to invert Player movement
     * since the camera rotates at 90-degree intervals.
     * Default camera placement should always be considered N
     */
    public enum CameraDirection
    {
        N, E, S, W
    }
}
