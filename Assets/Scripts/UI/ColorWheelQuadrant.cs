using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ColorWheelQuadrant : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private LevelManager _levelManager;
    private Image _quadrant;
    public Color highlightColor;
    public string paintType;

    private Color DEFAULT_COLOR = new Color32(180, 180, 180, 200);

    private void Start()
    {
        _levelManager = FindObjectOfType<LevelManager>();
        _quadrant = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _quadrant.color = highlightColor;
        _levelManager.ChangePaint(paintType);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _quadrant.color = DEFAULT_COLOR;
    }

    private void OnDisable()
    {
        _quadrant.color = DEFAULT_COLOR;
    }
}
