using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintLeftBG : MonoBehaviour
{
    private Image bg;

    private void Start()
    {
        bg = GetComponent<Image>();
    }

    public void SetColor(Color32 newColor)
    {
        // Make sure BG paint is not transparent
        newColor.a = 255;
        bg.color = newColor;
    }
}
