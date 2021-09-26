using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paints : MonoBehaviour
{
    // Identifiers for conditionals
    public const int RED_PAINT = 0;
    public const int GREEN_PAINT = 1;
    public const int BLACK_PAINT = 2;
    public const int ORANGE_PAINT = 3;
    public const int SPECIAL_PAINT = 4;

    // Paint colors
    public static Color32 red = new Color32(200, 62, 85, 100);
    public static Color32 green = new Color32(0, 255, 30, 235);
    public static Color32 black = new Color32(0, 0, 0, 100);
    public static Color32 orange = new Color32(255, 221, 0, 100);
    public static Color32 special = new Color32(45, 255, 240, 100);
}
