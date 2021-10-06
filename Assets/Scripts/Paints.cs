using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paints : MonoBehaviour
{
    // Identifiers for conditionals
    public const int RED_PAINT = 0;
    public const int GREEN_PAINT = 1;
    public const int ORANGE_PAINT = 2;
    public const int BLUE_PAINT = 3;
    //public const int BLACK_PAINT = 4;

    // Paint colors
    public static Color32 red = new Color32(250, 100, 82, 100);
    public static Color32 green = new Color32(180, 250, 82, 100);
    public static Color32 orange = new Color32(255, 243, 107, 100);
    public static Color32 blue = new Color32(82, 250, 239, 100);
    public static Color32 black = new Color32(0, 0, 0, 100);
}
