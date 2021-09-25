using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintLeftTextUpdater : MonoBehaviour
{
    private Text paintLeftText;

    private void Start()
    {
        paintLeftText = GetComponent<Text>();
    }

    public void SetPaintLeft(int paintLeft)
    {
        paintLeftText.text = paintLeft.ToString();
    }
}
