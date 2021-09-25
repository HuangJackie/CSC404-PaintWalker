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

    private void Start()
    {
        paintBucketIcon = GetComponent<Image>();
    }

    public void SetIcon(int paintType)
    {
        switch(paintType)
        {
            case Paints.GREEN_PAINT:
                paintBucketIcon.sprite = greenPaintBucket;
                break;
            default:
                paintBucketIcon.sprite = redPaintBucket;
                break;
        }
    }
}
