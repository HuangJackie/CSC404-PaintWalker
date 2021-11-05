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
        paintLeftText.text = String.Format("{0}", paintLeft.ToString());
    }
}
