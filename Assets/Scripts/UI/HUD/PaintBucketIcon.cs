using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintBucketIcon : MonoBehaviour
{
    [Header("Paint Sprites")]
    [SerializeField] private Image redPaintBucket;
    [SerializeField] private Image greenPaintBucket;
    [SerializeField] private Image yellowPaintBucket;
    [SerializeField] private Image bluePaintBucket;

    private Image currSelection;

    // For changing/restoring icon sizes
    private Vector2 originalSize;
    private Vector2 currSelectionSize;
    private float selectionGrowFactor = 1.3f;

    // For changing/restoring icon positions
    private Vector2 originalPos;
    private Vector3 selectionPosOffset;

    private void Start()
    {
        currSelection = null;
        originalPos = Vector2.zero;
        selectionPosOffset = new Vector3(-20f, 20f, 0f);

        originalSize = yellowPaintBucket.rectTransform.sizeDelta;
        currSelectionSize = originalSize * selectionGrowFactor;

        // Initially lower alpha for all icons
        SetAlpha(redPaintBucket, false);
        SetAlpha(greenPaintBucket, false);
        SetAlpha(yellowPaintBucket, false);
        SetAlpha(bluePaintBucket, false);
        SetIcon(GameConstants.YELLOW_PAINT);
    }

    // Change the active Icon type in the HUD UI
    // Should correspond to Player's currently selected paint
    public void SetIcon(int paintType)
    {
        // Revert currently selected paint icon to original size, alpha and pos first
        if (currSelection != null)
        {
            SetAlpha(currSelection, false);
            currSelection.rectTransform.sizeDelta = originalSize;
            if (originalPos != Vector2.zero)
            {
                currSelection.transform.position = originalPos;
            }
        }

        // Then choose next icon to enlarge and display as "selected"
        switch(paintType)
        {
            case GameConstants.GREEN_PAINT:
                currSelection = greenPaintBucket;
                originalPos = greenPaintBucket.transform.position;
                greenPaintBucket.rectTransform.sizeDelta = currSelectionSize;
                greenPaintBucket.transform.position += selectionPosOffset;
                break;

            case GameConstants.RED_PAINT:
                currSelection = redPaintBucket;
                originalPos = redPaintBucket.transform.position;
                redPaintBucket.rectTransform.sizeDelta = currSelectionSize;
                redPaintBucket.transform.position += selectionPosOffset;
                break;

            case GameConstants.YELLOW_PAINT:
                currSelection = yellowPaintBucket;
                originalPos = yellowPaintBucket.transform.position;
                yellowPaintBucket.rectTransform.sizeDelta = currSelectionSize;
                yellowPaintBucket.transform.position += selectionPosOffset;
                break;

            case GameConstants.BLUE_PAINT:
                currSelection = bluePaintBucket;
                originalPos = bluePaintBucket.transform.position;
                bluePaintBucket.rectTransform.sizeDelta = currSelectionSize;
                bluePaintBucket.transform.position += selectionPosOffset;
                break;
        }

        // Only set currSelection to full alpha if paintType
        // was valid enough to enter one of the switch cases above
        if (currSelection != null && IsValidPaint(paintType))
        {
            SetAlpha(currSelection, true);
        }
    }

    private void SetAlpha(Image icon, bool selected)
    {
        Color newColor = icon.color;
        newColor.a = selected ? 1f : 0.85f;
        icon.color = newColor;
    }

    // Return true if `paintType` is a valid paint type
    private bool IsValidPaint(int paintType)
    {
        return paintType >= 0 && paintType < 4;
    }
}
