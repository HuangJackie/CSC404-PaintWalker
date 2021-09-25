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
        paintQuantity.Add("Green", 0);   // Growing Platform
        paintQuantity.Add("Red", 0);     // Button
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void IncreasePaint(String paintColour, int quantity)
    {
        paintQuantity[paintColour] += quantity;
    }

    public void DecreaseCurrentSelectedPaint()
    {
        paintQuantity[currentSelectedColour]--;
    }

    public string GetCurrentlySelectedPaint()
    {
        return currentSelectedColour;
    }

    public bool HasEnoughPaint()
    {
        return paintQuantity[currentSelectedColour] > 0;
    }
}