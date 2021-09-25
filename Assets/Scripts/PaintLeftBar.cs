using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintLeftBar : MonoBehaviour
{
    private Slider slider;
    private Image fill;

    private void Start()
    {
        slider = GetComponent<Slider>();
        fill =  transform.Find("Fill").gameObject.GetComponent<Image>();
    }

    public void SetPaint(int paintLeft)
    {
        slider.value = paintLeft;
    }

    public void ChangePaint(int currPaint, int maxPaint, Color32 paintColor)
    {
        slider.value = currPaint;
        slider.maxValue = maxPaint;
        fill.color = paintColor;
    }
}
