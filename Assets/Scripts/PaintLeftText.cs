using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintLeftText : MonoBehaviour
{
    private Text paintLeftText;

    private void Start()
    {
        paintLeftText = GetComponent<Text>();
    }

    public void SetPaint(int paintLeft, int maxPaint)
    {
        paintLeftText.text = Math.Floor(paintLeft / maxPaint * 100f) + "%";
    }
}
