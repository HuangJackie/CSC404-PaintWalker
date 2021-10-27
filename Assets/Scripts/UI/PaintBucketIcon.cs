using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintBucketIcon : MonoBehaviour
{
    private Image paintBucketIcon;

    [Header("Paint Sprites")]
    [SerializeField] private Sprite redPaintBucket;
    [SerializeField] private Sprite greenPaintBucket;
    [SerializeField] private Sprite yellowPaintBucket;
    [SerializeField] private Sprite specialPaintBucket;

    private void Start()
    {
        paintBucketIcon = GetComponent<Image>();
    }

    public void SetIcon(int paintType)
    {
        switch(paintType)
        {
            case GameConstants.GREEN_PAINT:
                paintBucketIcon.sprite = greenPaintBucket;
                break;
            case GameConstants.RED_PAINT:
                paintBucketIcon.sprite = redPaintBucket;
                break;
            case GameConstants.YELLOW_PAINT:
                paintBucketIcon.sprite = yellowPaintBucket;
                break;
            case GameConstants.BLUE_PAINT:
                paintBucketIcon.sprite = specialPaintBucket;
                break;
            default:
                paintBucketIcon.sprite = redPaintBucket;
                break;
        }
    }
}
