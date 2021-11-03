using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class UpdateUI : MonoBehaviour
{
    private PaintBucketIcon paintIcon;
    private PaintLeftBar paintBar;
    private PaintLeftText paintText;
    private PaintLeftBG paintLeftBG;

    private CrosshairUI crosshairUI;
    private bool _isCrosshairActive;
    private Camera _camera;
    private TooltipObject _tooltipObject;

    [SerializeField] private GameObject infoTextBG;
    private PaintNeededText infoText;
    private bool _alreadyOverriden;

    private void Start()
    {
        paintIcon = FindObjectOfType<PaintBucketIcon>();
        paintBar = FindObjectOfType<PaintLeftBar>();
        paintText = FindObjectOfType<PaintLeftText>();
        paintLeftBG = FindObjectOfType<PaintLeftBG>();

        crosshairUI = FindObjectOfType<CrosshairUI>();
        _isCrosshairActive = false;
        _camera = FindObjectOfType<Camera>();
        _tooltipObject = null;

        infoText = FindObjectOfType<PaintNeededText>();
        infoTextBG.SetActive(false);

        ChangePaint(GameConstants.YELLOW_PAINT, 3);
    }

    private void Update()
    {
        if (_isCrosshairActive)
        {
            Ray ray = _camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hitInfo;
            TooltipObject newTooltipObject;
            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider.gameObject.TryGetComponent(out newTooltipObject))
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
                // Must set null otherwise UI won't appear onMouseOver, and can't activate special creature on click.
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

    public void ChangePaint(int paintType, int paintLeft)
    {
        Color32 paintColor;
        switch (paintType)
        {
            case GameConstants.GREEN_PAINT:
                paintColor = GameConstants.green;
                break;
            case GameConstants.RED_PAINT:
                paintColor = GameConstants.red;
                break;
            case GameConstants.YELLOW_PAINT:
                paintColor = GameConstants.yellow;
                break;
            case GameConstants.BLUE_PAINT:
                paintColor = GameConstants.blue;
                break;
            default:
                paintColor = GameConstants.red;
                break;
        }

        paintBar.ChangePaint(paintLeft, paintColor);
        paintText.SetPaint(paintLeft);
        paintIcon.SetIcon(paintType);
        paintLeftBG.SetColor(paintColor);
    }

    public void ClearUIInfoText()
    {
        infoTextBG.SetActive(false);
        _alreadyOverriden = false;
        infoText.SetPaintText("");
    }

    public void EnableCrosshairUI(bool isEnabled)
    {
        _isCrosshairActive = isEnabled;
        crosshairUI.EnableCrossHair(isEnabled);
    }
}