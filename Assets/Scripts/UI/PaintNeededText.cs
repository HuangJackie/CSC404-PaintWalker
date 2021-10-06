using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintNeededText : MonoBehaviour
{
    private Text paintLeftText;

    private void Awake()
    {
        paintLeftText = GetComponent<Text>();
    }

    public void SetPaintText(string paintNeededText)
    {
        paintLeftText.text = paintNeededText;
    }
}
