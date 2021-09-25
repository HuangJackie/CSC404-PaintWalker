using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private Dictionary<String, int> paintQuantity;
    private String currentSelectedColour;
    private bool _isColourSwitched;
    
    // Start is called before the first frame update
    void Start()
    {
        paintQuantity = new Dictionary<String, int>();
        paintQuantity.Add("Special", 2); // Goal
        paintQuantity.Add("Green", 5);   // Growing Platform
        paintQuantity.Add("Red", 10);     // Button

        currentSelectedColour = "Red";
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(GetCurrentlySelectedPaint());
        Debug.Log(GetPaintQuantity());
    }
    
    public void IncreasePaint(String paintColour, int quantity)
    {
        paintQuantity[paintColour] += quantity;
    }

    public void DecreaseCurrentSelectedPaint(int amount)
    {
        paintQuantity[currentSelectedColour] -= amount;
    }

    public string GetCurrentlySelectedPaint()
    {
        return currentSelectedColour;
    }

    public void SetCurrentlySelectedPaint(string color)
    {
        currentSelectedColour = color;
    }

    public bool HasEnoughPaint()
    {
        return paintQuantity[currentSelectedColour] > 0;
    }

    public int GetPaintQuantity()
    {
        string color = GetCurrentlySelectedPaint();
        return paintQuantity[color];
    }
}