using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintBucketIcon : MonoBehaviour
{
    private Image paintBucketIcon;

    [Header("Paint Sprites")]
    [SerializeField] private Image redPaintBucket;
    [SerializeField] private Image greenPaintBucket;
    [SerializeField] private Image yellowPaintBucket;
    [SerializeField] private Image specialPaintBucket;
    private Vector2 newSize;
    private Vector2 originalSize;
    private Image cur_selection;
    private Vector2 old_pos;
    private Vector2 new_pos;

    private void Awake()
    {
        originalSize = new Vector2(greenPaintBucket.rectTransform.sizeDelta.x, greenPaintBucket.rectTransform.sizeDelta.y);
        newSize = new Vector2(greenPaintBucket.rectTransform.sizeDelta.x * 1.3f, greenPaintBucket.rectTransform.sizeDelta.y * 1.3f);
        cur_selection = yellowPaintBucket;
        paintBucketIcon = GetComponent<Image>();
    }

    public void SetIcon(int paintType)
    {
        cur_selection.rectTransform.sizeDelta = originalSize;
        if (old_pos != Vector2.zero)
        {
            cur_selection.transform.position = old_pos;
        }
        switch(paintType)
        {
            case GameConstants.GREEN_PAINT:
                greenPaintBucket.rectTransform.sizeDelta = newSize;
                cur_selection = greenPaintBucket;
                old_pos = new Vector2(greenPaintBucket.transform.position.x, greenPaintBucket.transform.position.y);
                new_pos = new Vector2(greenPaintBucket.transform.position.x - 25, greenPaintBucket.transform.position.y + 25);
                greenPaintBucket.transform.position = new_pos;
                break;
            case GameConstants.RED_PAINT:
                redPaintBucket.rectTransform.sizeDelta = newSize;
                cur_selection = redPaintBucket;
                old_pos = new Vector2(redPaintBucket.transform.position.x, redPaintBucket.transform.position.y);
                new_pos = new Vector2(redPaintBucket.transform.position.x - 25, redPaintBucket.transform.position.y + 25);
                redPaintBucket.transform.position = new_pos;
                break;
            case GameConstants.YELLOW_PAINT:
                print("here");
                yellowPaintBucket.rectTransform.sizeDelta = newSize;
                cur_selection = yellowPaintBucket;
                old_pos = new Vector2(yellowPaintBucket.transform.position.x, yellowPaintBucket.transform.position.y);
                print(old_pos);
                new_pos = new Vector2(yellowPaintBucket.transform.position.x - 25, yellowPaintBucket.transform.position.y + 20);
                yellowPaintBucket.transform.position = new_pos;
                break;
            case GameConstants.BLUE_PAINT:
                specialPaintBucket.rectTransform.sizeDelta = newSize;
                cur_selection = specialPaintBucket;
                old_pos = new Vector2(specialPaintBucket.transform.position.x, specialPaintBucket.transform.position.y);
                new_pos = new Vector2(specialPaintBucket.transform.position.x - 25, specialPaintBucket.transform.position.y + 25);
                specialPaintBucket.transform.position = new_pos;
                break;
        }
    }
}
