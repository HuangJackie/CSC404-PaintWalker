using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateUI : MonoBehaviour
{
    private PaintBucketIcon paintIcon;
    private PaintLeftBar paintBar;
    private PaintLeftText paintText;
    private PaintNeededText infoText;

    private bool _alreadyOverriden;

    private void Start()
    {
        paintIcon = FindObjectOfType<PaintBucketIcon>();
        paintBar = FindObjectOfType<PaintLeftBar>();
        paintText = FindObjectOfType<PaintLeftText>();
        infoText = FindObjectOfType<PaintNeededText>();
        ChangePaint(Paints.ORANGE_PAINT, 3);
    }

    public void SetInfoText(string text, bool preventOverride = false)
    {

        if (!_alreadyOverriden)
        {
            _alreadyOverriden = preventOverride;
            infoText.SetPaintText(text);
        }
    }

    public void SetPaint(int paintLeft)
    {
        paintBar.SetPaint(paintLeft);
        paintText.SetPaint(paintLeft);
    }

    public void ChangePaint(int paintType, int paintLeft)
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
            case Paints.BLUE_PAINT:
                paintColor = Paints.blue;
                break;
            default:
                paintColor = Paints.red;
                break;
        }

        paintBar.ChangePaint(paintLeft, paintColor);
        paintText.SetPaint(paintLeft);
        paintIcon.SetIcon(paintType);
    }

    public void ClearUIInfoText()
    {
        _alreadyOverriden = false;
        infoText.SetPaintText("");
    }
}