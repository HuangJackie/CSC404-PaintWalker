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
        paintQuantity.Add("Green", 5); // Growing Platform
        paintQuantity.Add("Red", 15); // Button
        paintQuantity.Add("Black", 0); // Weighted Platform
        paintQuantity.Add("Orange", 5); // Walking

        currentSelectedColour = "Orange";
        _updateUI = FindObjectOfType<UpdateUI>();
        _updateUI.SetPaint(paintQuantity["Orange"], 10);
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
                    _updateUI.ChangePaint(Paints.RED_PAINT, paintQuantity[currentSelectedColour], 10);
                    break;
                case "Red":
                    currentSelectedColour = "Green";
                    _updateUI.ChangePaint(Paints.GREEN_PAINT, paintQuantity[currentSelectedColour], 10);
                    break;
                case "Green":
                    currentSelectedColour = "Black";
                    _updateUI.ChangePaint(Paints.BLACK_PAINT, paintQuantity[currentSelectedColour], 10);
                    break;
                case "Black":
                    currentSelectedColour = "Orange";
                    _updateUI.ChangePaint(Paints.ORANGE_PAINT, paintQuantity[currentSelectedColour], 10);
                    break;
                case "Orange":
                    currentSelectedColour = "Special";
                    _updateUI.ChangePaint(Paints.SPECIAL_PAINT, paintQuantity[currentSelectedColour], 10);
                    break;
            }
            // ui.SelectColour(currentSelectedColour);
        }
    }

    public void IncreasePaint(String paintColour, int quantity)
    {
        paintQuantity[paintColour] += quantity;
        if (currentSelectedColour == paintColour)
        {
            _updateUI.SetPaint(paintQuantity[paintColour], 10);
        }
    }

    public void DecreaseCurrentSelectedPaint(int amount)
    {
        paintQuantity[currentSelectedColour] -= amount;
        _updateUI.SetPaint(paintQuantity[currentSelectedColour], 10);
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