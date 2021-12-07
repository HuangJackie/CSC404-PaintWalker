using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UI;
using UnityEngine;
using static GameConstants;

public class UpdateUI : MonoBehaviour
{
    // General components
    private Camera _camera;

    // Top-left HUD components
    private PaintBucketIcon paintIcon;
    private PaintLeftBar paintBar;
    private PaintLeftText paintText;

    private YellowDotText yellowDotText;
    private RedDotText redDotText;
    private BlueDotText blueDotText;
    private GreenDotText greenDotText;

    // Info displaying components
    [SerializeField] private GameObject infoTextBG;
    private PaintNeededText infoText;
    private TooltipObject _tooltipObject;
    private bool _alreadyOverriden;

    // Crosshair components
    private CrosshairUI crosshairUI;
    private PanningArrowUI panningArrowUI;
    private bool _isCrosshairActive;

    private void Awake()
    {
        // Init general objects
        _camera = FindObjectOfType<Camera>();
        _tooltipObject = null;

        // Init top-left HUD objects
        paintIcon = FindObjectOfType<PaintBucketIcon>();
        paintBar = FindObjectOfType<PaintLeftBar>();
        paintText = FindObjectOfType<PaintLeftText>();

        yellowDotText = FindObjectOfType<YellowDotText>();
        redDotText = FindObjectOfType<RedDotText>();
        blueDotText = FindObjectOfType<BlueDotText>();
        greenDotText = FindObjectOfType<GreenDotText>();

        // Init crosshair
        crosshairUI = FindObjectOfType<CrosshairUI>();
        panningArrowUI = FindObjectOfType<PanningArrowUI>();
        _isCrosshairActive = false;

        // Init corsshair
        infoText = FindObjectOfType<PaintNeededText>();
        infoTextBG.SetActive(false);
    }

    private void Update()
    {
        if (_isCrosshairActive)
        {
            Ray ray = _camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hitInfo;
            TooltipObject newTooltipObject;

            if (Physics.Raycast(ray, out hitInfo) &&
                hitInfo.collider.gameObject.TryGetComponent(out newTooltipObject))
            {
                if (_tooltipObject != null && newTooltipObject != _tooltipObject)
                {
                    _tooltipObject.OnExitTooltip();
                }

                _tooltipObject = newTooltipObject;
                _tooltipObject.OnDisplayTooltip();
            }
            else if (_tooltipObject != null)
            {
                _tooltipObject.OnExitTooltip();

                // Must set null otherwise UI won't appear onMouseOver,
                // and can't activate special creature on click.
                // Since it causes the UI to keep being wiped.
                _tooltipObject = null;
            }
        }
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

    public void WipeInfoText()
    {
        // If resetting the text with "", then remove the BG as well
        infoText.SetPaintText("");
        infoTextBG.SetActive(false);
    }

    public void SetPaint(int paintLeft)
    {
        paintBar.SetPaint(paintLeft);
        paintText.SetPaint(paintLeft);
    }

    public void InitPaintInfoText(int yellow, int red, int blue, int green)
    {
        yellowDotText.SetPaint(yellow);
        redDotText.SetPaint(red);
        blueDotText.SetPaint(blue);
        greenDotText.SetPaint(green);
    }

    public void UpdatePaintInfoText(Paints paintType, int cur_amount)
    {
        switch (paintType) {
            case Paints.Yellow:
                yellowDotText.SetPaint(cur_amount);
                break;
            case Paints.Red:
                redDotText.SetPaint(cur_amount);
                break;
            case Paints.Green:
                greenDotText.SetPaint(cur_amount);
                break;
            case Paints.Blue:
                blueDotText.SetPaint(cur_amount);
                break;
        }
    }

    public void ChangePaint(Paints paintType, int paintLeft)
    {
        Color32 paintColor;
        switch (paintType)
        {
            case Paints.Yellow:
                paintColor = GameConstants.Yellow;
                break;
            case Paints.Red:
                paintColor = GameConstants.Red;
                break;
            case Paints.Green:
                paintColor = GameConstants.Green;
                break;
            case Paints.Blue:
                paintColor = GameConstants.Blue;
                break;
            default:
                paintColor = GameConstants.Red;
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

    public void EnablePanningUI(bool isEnabled)
    {
        _isCrosshairActive = isEnabled;
        crosshairUI.EnableCrossHair(isEnabled);
        panningArrowUI.EnablePanningArrows(isEnabled);
    }
}