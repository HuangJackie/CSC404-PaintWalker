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
    [SerializeField] private Image specialPaintBucket;

    private Image currSelection;

    // For changing/restoring icon sizes
    private Vector2 originalSize;
    private Vector2 currSelectionSize;
    private float selectionGrowFactor = 1.4f;

    // For changing/restoring icon positions
    private Vector2 originalPos;
    private Vector3 selectionPosOffset;

    private void Awake()
    {
        currSelection = null;
        originalPos = Vector2.zero;
        selectionPosOffset = new Vector3(-18f, 15f, 0f);

        originalSize = new Vector2(greenPaintBucket.rectTransform.sizeDelta.x,
                                   greenPaintBucket.rectTransform.sizeDelta.y);
        currSelectionSize = new Vector2(greenPaintBucket.rectTransform.sizeDelta.x * selectionGrowFactor,
                                        greenPaintBucket.rectTransform.sizeDelta.y * selectionGrowFactor);
        
        SetIcon(GameConstants.YELLOW_PAINT);
    }

    // Change the active Icon type in the HUD UI
    // Should correspond to Player's currently selected paint
    public void SetIcon(int paintType)
    {
        // Revert currently selected paint icon to original size and pos first
        if (currSelection != null)
        {
            currSelection.rectTransform.sizeDelta = originalSize;
        }
        if (originalPos != Vector2.zero)
        {
            currSelection.transform.position = originalPos;
        }

        // Then choose next icon to enlarge and display as "selected"
        switch(paintType)
        {
            case GameConstants.GREEN_PAINT:
                currSelection = greenPaintBucket;
                greenPaintBucket.rectTransform.sizeDelta = currSelectionSize;
                originalPos = new Vector2(greenPaintBucket.transform.position.x,
                                          greenPaintBucket.transform.position.y);
                greenPaintBucket.transform.position += selectionPosOffset;
                break;

            case GameConstants.RED_PAINT:
                currSelection = redPaintBucket;
                redPaintBucket.rectTransform.sizeDelta = currSelectionSize;
                originalPos = new Vector2(redPaintBucket.transform.position.x,
                                          redPaintBucket.transform.position.y);
                redPaintBucket.transform.position += selectionPosOffset;
                break;

            case GameConstants.YELLOW_PAINT:
                currSelection = yellowPaintBucket;
                yellowPaintBucket.rectTransform.sizeDelta = currSelectionSize;
                originalPos = new Vector2(yellowPaintBucket.transform.position.x,
                                          yellowPaintBucket.transform.position.y);
                yellowPaintBucket.transform.position += selectionPosOffset;
                break;

            case GameConstants.BLUE_PAINT:
                currSelection = specialPaintBucket;
                specialPaintBucket.rectTransform.sizeDelta = currSelectionSize;
                originalPos = new Vector2(specialPaintBucket.transform.position.x,
                                          specialPaintBucket.transform.position.y);
                specialPaintBucket.transform.position += selectionPosOffset;
                break;
        }
    }
}
