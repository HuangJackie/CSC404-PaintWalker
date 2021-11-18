using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedDotText : MonoBehaviour
{
    private Text redDotText;

    private void Awake()
    {
        redDotText = GetComponent<Text>();
    }

    public void SetPaint(int paintLeft)
    {
        redDotText.text = String.Format("{0}", paintLeft.ToString());
    }
}
