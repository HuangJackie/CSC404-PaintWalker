using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paints : MonoBehaviour
{
    // Identifiers for conditionals
    public const int RED_PAINT = 0;
    public const int GREEN_PAINT = 1;
    public const int YELLOW_PAINT = 2;
    public const int BLUE_PAINT = 3;

    // Paint colors
    public static Color32 red = new Color32(241, 95, 62, 255);
    public static Color32 green = new Color32(166, 191, 75, 255);
    public static Color32 yellow = new Color32(242, 191, 61, 255);
    public static Color32 blue = new Color32(140, 210, 205, 255);
}
