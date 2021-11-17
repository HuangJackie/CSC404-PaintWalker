using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GreenDotText : MonoBehaviour
{
    private Text greenDotText;

    private void Awake()
    {
        greenDotText = GetComponent<Text>();
    }

    public void SetPaint(int paintLeft)
    {
        greenDotText.text = String.Format("{0}", paintLeft.ToString());
    }
}
