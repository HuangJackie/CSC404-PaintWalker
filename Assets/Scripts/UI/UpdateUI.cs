using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateUI : MonoBehaviour
{
    private PaintBucketIcon paintIcon;
    private PaintLeftBar paintBar;
    private PaintLeftText paintText;

    [SerializeField] private GameObject infoTextBG;
    private PaintNeededText infoText;
    private bool _alreadyOverriden;

    private void Start()
    {
        paintIcon = FindObjectOfType<PaintBucketIcon>();
        paintBar = FindObjectOfType<PaintLeftBar>();
        paintText = FindObjectOfType<PaintLeftText>();
        infoText = FindObjectOfType<PaintNeededText>();
        infoTextBG.SetActive(false);

        ChangePaint(Paints.ORANGE_PAINT, 3);
    }

    public void SetInfoText(string text, bool preventOverride = false)
    {
        if (!_alreadyOverriden)
        {
            // If resetting the text with "", then remove the BG as well
            if (text == "")
                infoTextBG.SetActive(false);
            else
                infoTextBG.SetActive(true);

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
        infoTextBG.SetActive(false);
        _alreadyOverriden = false;
        infoText.SetPaintText("");
    }
}