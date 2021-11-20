using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YellowDotText : MonoBehaviour
{
    private Text yellowDotText;

    private void Awake()
    {
        yellowDotText = GetComponent<Text>();
    }

    public void SetPaint(int paintLeft)
    {
        yellowDotText.text = String.Format("{0}", paintLeft.ToString());
    }
}
