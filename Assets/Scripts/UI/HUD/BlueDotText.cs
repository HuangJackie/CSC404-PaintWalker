using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlueDotText : MonoBehaviour
{
    private Text blueDotText;

    private void Awake()
    {
        blueDotText = GetComponent<Text>();
    }

    public void SetPaint(int paintLeft)
    {
        blueDotText.text = String.Format("{0}", paintLeft.ToString());
    }
}
