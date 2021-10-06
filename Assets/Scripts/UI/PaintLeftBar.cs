using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintLeftBar : MonoBehaviour
{
    private Slider slider;
    private Image fill;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        fill =  transform.Find("Fill").gameObject.GetComponent<Image>();
    }

    public void SetPaint(int paintLeft)
    {
        slider.value = paintLeft;
    }

    public void ChangePaint(int currPaint, Color32 paintColor)
    {
        slider.value = currPaint;
        slider.maxValue = 25;
        fill.color = paintColor;
    }
}
