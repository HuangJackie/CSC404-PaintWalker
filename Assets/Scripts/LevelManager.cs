using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private Dictionary<String, int> paintQuantity;
    private String currentSelectedColour;
    private Color currentSelectedColourClass;
    private bool _isColourSwitched;
    public bool dev_mode;

    private UpdateUI _updateUI;
    private bool _isExitActive = false;
    private bool _isPanning = false;

    void Start()
    {
        paintQuantity = new Dictionary<String, int>();
        paintQuantity.Add("Blue", 0); // Freezes Platform
        paintQuantity.Add("Green", 0); // Growing Platform
        paintQuantity.Add("Red", 0); // Drops Platform
        paintQuantity.Add("Yellow", 4); // Raises Platform
        
        if (dev_mode)
        {
            paintQuantity["Blue"] = 30;
            paintQuantity["Green"] = 30;
            paintQuantity["Red"] = 30;
            paintQuantity["Yellow"] = 30;
        }

        currentSelectedColour = "Yellow";
        currentSelectedColourClass = Paints.yellow;
        _updateUI = FindObjectOfType<UpdateUI>(); // Auto-sets yellow to 3/10
    }

    // Update is called once per frame
    void Update()
    {
        _isColourSwitched = Input.GetButtonDown("Fire2");
        if (_isColourSwitched)
        {
            switch (currentSelectedColour)
            {
                case "Blue":
                    currentSelectedColour = "Red";
                    currentSelectedColourClass = Paints.red;
                    _updateUI.ChangePaint(Paints.RED_PAINT, paintQuantity[currentSelectedColour]);
                    break;
                case "Red":
                    currentSelectedColour = "Green";
                    currentSelectedColourClass = Paints.green;
                    _updateUI.ChangePaint(Paints.GREEN_PAINT, paintQuantity[currentSelectedColour]);
                    break;
                case "Green":
                    currentSelectedColour = "Yellow";
                    currentSelectedColourClass = Paints.yellow;
                    _updateUI.ChangePaint(Paints.YELLOW_PAINT, paintQuantity[currentSelectedColour]);
                    break;
                case "Yellow":
                    currentSelectedColour = "Blue";
                    currentSelectedColourClass = Paints.blue;
                    _updateUI.ChangePaint(Paints.BLUE_PAINT, paintQuantity[currentSelectedColour]);
                    break;
            }
        }
    }

    IEnumerator CheckPaintQuantity()
    {
        yield return new WaitForSeconds(0.5f); // Takes time to collect the paint and replenish
        if (HasNoPaintLeft())
        {
            UpdateNoPaintLeftUI();
        }
    }

    public void SetIsPanning(bool isPanning)
    {
        _isPanning = isPanning;
    }

    public bool IsPanning()
    {
        return _isPanning;
    }

    public void IncreasePaint(String paintColour, int quantity)
    {
        int amount = quantity;
        paintQuantity[paintColour] += amount;
        if (currentSelectedColour == paintColour)
        {
            _updateUI.SetPaint(paintQuantity[paintColour]);
        }
    }

    public void DecreasePaint(String paintColour, int quantity)
    {
        int amount = quantity;
        paintQuantity[paintColour] -= amount;
        if (currentSelectedColour == paintColour)
        {
            _updateUI.SetPaint(paintQuantity[paintColour]);
        }

        StopCoroutine("CheckPaintQuantity"); // Stop existing coroutine.
        StartCoroutine("CheckPaintQuantity");
    }

    private void ClearUIInfoText()
    {
        _updateUI.ClearUIInfoText();
    }

    public void DecreaseCurrentSelectedPaint(int amount)
    {
        paintQuantity[currentSelectedColour] -= amount;
        _updateUI.SetPaint(paintQuantity[currentSelectedColour]);
        StopCoroutine("CheckPaintQuantity"); // Stop existing coroutine.
        StartCoroutine("CheckPaintQuantity");
    }

    public bool HasNoPaintLeft()
    {
        foreach (int paintQuantityValue in paintQuantity.Values)
        {
            if (paintQuantityValue != 0)
            {
                return false;
            }
        }

        return true;
    }

    public void UpdateNoPaintLeftUI()
    {
        _updateUI.SetInfoText("No Paint Left :(", true);
    }

    public string GetCurrentlySelectedPaint()
    {
        return currentSelectedColour;
    }

    public Color GetCurrentlySelectedPaintClass()
    {
        return currentSelectedColourClass;
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

    public int GetPaintQuantity(string colour)
    {
        return paintQuantity[colour];
    }

    public bool IsExitActive()
    {
        return _isExitActive;
    }

    public void SetExitActive(bool isActive)
    {
        _isExitActive = isActive;
    }
}