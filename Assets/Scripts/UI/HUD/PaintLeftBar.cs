using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaintLeftBar : MonoBehaviour
{
    private Slider slider;
    private Image fill;
    private LevelManager _levelManager;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        fill =  transform.Find("Fill").gameObject.GetComponent<Image>();
    }

    private void Start()
    {
        _levelManager = FindObjectOfType<LevelManager>();
    }

    public void SetPaint(int paintLeft)
    {
        //print("set paint to" + paintLeft.ToString());
        slider.value = paintLeft;
    }

    public void ChangePaint(int currPaint, Color32 paintColor)
    {
        slider.value = currPaint;
        if (currPaint > 100)
        {
            slider.maxValue = 120;
        } else
        {
            slider.maxValue = 100;
        }
        fill.color = paintColor;
    }

    //public void IncreaseMax(string color)
    //{
    //    slider.maxValue = _levelManager.GetPaintQuantity();
    //}
}
