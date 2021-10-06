using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintLeftText : MonoBehaviour
{
    private Text paintLeftText;

    private void Awake()
    {
        paintLeftText = GetComponent<Text>();
    }

    public void SetPaint(int paintLeft)
    {
        int maxPaint = 25;
        //Int display
        paintLeftText.text = String.Format("{0} / {1}", paintLeft.ToString(), maxPaint.ToString());
        //Percentage display
        //paintLeftText.text = Math.Floor((paintLeft + 0f) / maxPaint * 100f) + "%";
    }
}
