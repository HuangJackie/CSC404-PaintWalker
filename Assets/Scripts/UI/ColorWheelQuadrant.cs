using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static GameConstants;

public class ColorWheelQuadrant : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private LevelManager _levelManager;
    private Image _quadrant;
    private ControllerUtil _controllerUtil;
    public Color highlightColor;
    public Paints paint;
    public string location;

    private Color DEFAULT_COLOR = new Color32(180, 180, 180, 200);
    
    /**
     * If the joystick axis are within this delta range than unselect.
     */
    private const float DeltaToUndoSelect = 0.3f;

    private void Start()
    {
        _levelManager = FindObjectOfType<LevelManager>();
        _controllerUtil = FindObjectOfType<ControllerUtil>();
        _quadrant = GetComponent<Image>();
    }

    private void Update()
    {
        ListenForControllerInput();
    }

    private void ListenForControllerInput()
    {
        if (_controllerUtil.GetColourWheelPressed())
        {
            float ySelect = _controllerUtil.GetColourWheelSelectYAxis();
            float xSelect = _controllerUtil.GetColourWheelSelectXAxis();

            UnSwitchPaintColour();
            if (IsWithinRange(xSelect, DeltaToUndoSelect) &&
                IsWithinRange(ySelect, DeltaToUndoSelect))
            {
                return;
            }

            switch (location)
            {
                case GameConstants.NorthwestQuadrant:
                    if (ySelect < 0 && xSelect > 0)
                    {
                        SwitchPaintColour();
                    }
                    break;
                case GameConstants.SouthwestQuadrant:
                    if (ySelect < 0 && xSelect < 0)
                    {
                        SwitchPaintColour();
                    }
                    break;
                case GameConstants.NortheastQuadrant:
                    if (ySelect > 0 && xSelect > 0)
                    {
                        SwitchPaintColour();
                    }
                    break;
                case GameConstants.SoutheastQuadrant:
                    if (ySelect > 0 && xSelect < 0)
                    {
                        SwitchPaintColour();
                    }
                    break;
                default:
                    Debug.LogError("Invalid Colour Wheel Quadrant");
                    break;
            }
        }
    }

    private bool IsWithinRange(float toCompare, float delta)
    {
        return -delta <= toCompare && toCompare <= delta;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SwitchPaintColour();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UnSwitchPaintColour();
    }

    private void SwitchPaintColour()
    {
        _quadrant.color = highlightColor;
        _levelManager.ChangePaint(paint);
    }

    private void UnSwitchPaintColour()
    {
        _quadrant.color = DEFAULT_COLOR;
    }

    private void OnDisable()
    {
        _quadrant.color = DEFAULT_COLOR;
    }
}