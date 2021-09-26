using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private Dictionary<String, int> paintQuantity;
    private String currentSelectedColour;
    private bool _isColourSwitched;

    private UpdateUI _updateUI;
    
    // Start is called before the first frame update
    void Start()
    {
        paintQuantity = new Dictionary<String, int>();
        paintQuantity.Add("Special", 2); // Goal
        paintQuantity.Add("Green", 5);   // Growing Platform
        paintQuantity.Add("Red", 15);     // Button
        paintQuantity.Add("Black", 0);     // Weighted Platform

        currentSelectedColour = "Red";
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(GetCurrentlySelectedPaint());
        // Debug.Log(GetPaintQuantity());
        
        _isColourSwitched = Input.GetButtonDown("Fire2");
        // Debug.Log(currentSelectedColour);
        if (_isColourSwitched)
        {
            switch (currentSelectedColour)
            {
                case "Special":
                    currentSelectedColour = "Red";
                    break;
                case "Red":
                    currentSelectedColour = "Green";
                    break;
                case "Green":
                    currentSelectedColour = "Special";
                    break;
            }
            // ui.SelectColour(currentSelectedColour);
        }
    }
    
    public void IncreasePaint(String paintColour, int quantity)
    {
        paintQuantity[paintColour] += quantity;
    }

    public void DecreaseCurrentSelectedPaint(int amount)
    {
        paintQuantity[currentSelectedColour] -= amount;
        // _updateUI.SetPaint(paintQuantity[currentSelectedColour], 10);

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