using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateUI : MonoBehaviour
{
    private PaintBucketIcon paintIcon;
    private PaintLeftBar paintBar;
    private PaintLeftText paintText;

    private void Start()
    {
        paintIcon = FindObjectOfType<PaintBucketIcon>();
        paintBar = FindObjectOfType<PaintLeftBar>();
        paintText = FindObjectOfType<PaintLeftText>();
        ChangePaint(Paints.ORANGE_PAINT, 3, 10);
    }

    public void SetPaint(int paintLeft, int maxPaint)
    {
        paintBar.SetPaint(paintLeft);
        paintText.SetPaint(paintLeft, maxPaint);
    }

    public void ChangePaint(int paintType, int paintLeft, int maxPaint)
    {
        Color32 paintColor;
        switch (paintType)
        {
            case Paints.GREEN_PAINT:
                paintColor = Paints.green;
                break;
            case Paints.RED_PAINT:
                paintColor = Paints.red;
                break;
            case Paints.ORANGE_PAINT:
                paintColor = Paints.orange;
                break;
            case Paints.BLACK_PAINT:
                paintColor = Paints.black;
                break;
            case Paints.SPECIAL_PAINT:
                paintColor = Paints.special;
                break;
            default:
                paintColor = Paints.red;
                break;
        }

        paintBar.ChangePaint(paintLeft, maxPaint, paintColor);
        paintText.SetPaint(paintLeft, maxPaint);
        paintIcon.SetIcon(paintType);
    }
}