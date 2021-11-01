using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private Dictionary<String, int> paintQuantity;
    private String currentSelectedColour;
    private Color currentSelectedColourClass;
    private bool _isColourSwitched;
    public bool dev_mode;
    public bool freeze_player;

    public RedoCommandHandler redoCommandHandler = new RedoCommandHandler();
    public Player player_script;
    private PaintBrush playerPaintBrush;
    private PaintBottle playerPaintBottle;

    private UpdateUI _updateUI;
    private bool _isExitActive;
    private bool _isPanning;
    private bool _paintSelectionUIDisplayed;

    private SoundManager _colourChangeSoundManager = new SoundManager();

    private Queue<Func<IEnumerator>> actionQueue = new Queue<Func<IEnumerator>>();
    private PaintSelectionUI _paintSelectionUI;

    void Start()
    {
        StartCoroutine(ManageCoroutines());

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

        freeze_player = false;
        currentSelectedColour = "Yellow";
        currentSelectedColourClass = GameConstants.yellow;
        _updateUI = FindObjectOfType<UpdateUI>(); // Auto-sets yellow to 3/10

        playerPaintBrush = FindObjectOfType<PaintBrush>();
        playerPaintBottle = FindObjectOfType<PaintBottle>();

        _colourChangeSoundManager.SetAudioSources(GetComponents<AudioSource>());

        _paintSelectionUIDisplayed = false;
        _paintSelectionUI = FindObjectOfType<PaintSelectionUI>();
    }

    IEnumerator ManageCoroutines()
    {
        while (true)
        {
            while (actionQueue.Count > 0)
            {
                //Func<IEnumerator> next_co = actionQueue.Dequeue();
                //Coroutine co = StartCoroutine(next_co());
                yield return StartCoroutine(actionQueue.Dequeue()());
            }

            yield return null;
        }
    }

    public void EnqueueAction(Func<IEnumerator> action)
    {
        //For debugging actions getting stuck
        //print(actionQueue.Count);
        //foreach (Func<IEnumerator> i in actionQueue)
        //{
        //    print(i.Method.Name);
        //}
        actionQueue.Enqueue(action);
    }

    // Update is called once per frame
    void Update()
    {
        if (freeze_player)
        {
            return;
        }

        if (Input.GetButtonDown("Undo"))
        {
            Undo();
        }
    }

    public void ChangePaint(string paintType)
    {
        _colourChangeSoundManager.PlayRandom();
        switch (paintType)
        {
            case "Red":
                currentSelectedColour = "Red";
                currentSelectedColourClass = GameConstants.red;

                _updateUI.ChangePaint(GameConstants.RED_PAINT, paintQuantity[currentSelectedColour]);
                playerPaintBrush.SetColor(GameConstants.red);
                playerPaintBottle.SetColor(GameConstants.red);
                break;

            case "Green":
                currentSelectedColour = "Green";
                currentSelectedColourClass = GameConstants.green;

                _updateUI.ChangePaint(GameConstants.GREEN_PAINT, paintQuantity[currentSelectedColour]);
                playerPaintBrush.SetColor(GameConstants.green);
                playerPaintBottle.SetColor(GameConstants.green);
                break;

            case "Yellow":
                currentSelectedColour = "Yellow";
                currentSelectedColourClass = GameConstants.yellow;

                _updateUI.ChangePaint(GameConstants.YELLOW_PAINT, paintQuantity[currentSelectedColour]);
                playerPaintBrush.SetColor(GameConstants.yellow);
                playerPaintBottle.SetColor(GameConstants.yellow);
                break;

            case "Blue":
                currentSelectedColour = "Blue";
                currentSelectedColourClass = GameConstants.blue;

                _updateUI.ChangePaint(GameConstants.BLUE_PAINT, paintQuantity[currentSelectedColour]);
                playerPaintBrush.SetColor(GameConstants.blue);
                playerPaintBottle.SetColor(GameConstants.blue);
                break;
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

    public MoveRedo GetLasestGameState()
    {
        return redoCommandHandler.LatestCommand() as MoveRedo;
    }

    public void Undo()
    {
        actionQueue.Clear();
        StopAllCoroutines();
        StartCoroutine(ManageCoroutines());
        redoCommandHandler.Undo();
        if (player_script != null)
        {
            player_script.UpdateTargetLocation(player_script.gameObject.transform.position);
        }
    }

    public void SetIsPanning(bool isPanning)
    {
        _updateUI.EnableCrosshairUI(isPanning);
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
        MoveRedo lastCommand = redoCommandHandler.LatestCommand() as MoveRedo;
        if (lastCommand)
        {
            lastCommand.RecordPaintSpent(paintColour, -quantity);
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
        MoveRedo lastCommand = redoCommandHandler.LatestCommand() as MoveRedo;
        if (lastCommand)
        {
            lastCommand.RecordPaintSpent(paintColour, quantity);
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
        _updateUI.SetInfoText("No Paint Left", true);
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

    public void SetPaintSelectionUIDisplayed(bool isDisplayed)
    {
        _paintSelectionUIDisplayed = isDisplayed;
    }

    public bool IsPaintSelectionUIDisplayed()
    {
        return _paintSelectionUIDisplayed;
    }

    public void RefreshPaintSelectionUI()
    {
        if (_paintSelectionUIDisplayed)
        {
            _paintSelectionUI.ClosePaintSelectionUI();
            _paintSelectionUI.DisplayPaintSelectionUI();
        }
    }
}