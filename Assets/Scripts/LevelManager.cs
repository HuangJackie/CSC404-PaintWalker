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
    private bool _isExitActive = false;
    private bool _isPanning = false;
    private bool _isRunningCoroutine = true;

    private SoundManager _colourChangeSoundManager = new SoundManager();

    private Queue<Func<IEnumerator>> actionQueue = new Queue<Func<IEnumerator>>();

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
        currentSelectedColourClass = Paints.yellow;
        _updateUI = FindObjectOfType<UpdateUI>(); // Auto-sets yellow to 3/10

        playerPaintBrush = FindObjectOfType<PaintBrush>();
        playerPaintBottle = FindObjectOfType<PaintBottle>();

        _colourChangeSoundManager.SetAudioSources(GetComponents<AudioSource>());
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
        actionQueue.Enqueue(action);
    }

    // Update is called once per frame
    void Update()
    {
        if (freeze_player)
        {
            return;
        }

        _isColourSwitched = Input.GetButtonDown("Fire2");
        if (_isColourSwitched)
        {
            _colourChangeSoundManager.PlayRandom();
            switch (currentSelectedColour)
            {
                case "Blue":
                    currentSelectedColour = "Red";
                    currentSelectedColourClass = Paints.red;

                    _updateUI.ChangePaint(Paints.RED_PAINT, paintQuantity[currentSelectedColour]);
                    playerPaintBrush.SetColor(Paints.red);
                    playerPaintBottle.SetColor(Paints.red);
                    break;

                case "Red":
                    currentSelectedColour = "Green";
                    currentSelectedColourClass = Paints.green;

                    _updateUI.ChangePaint(Paints.GREEN_PAINT, paintQuantity[currentSelectedColour]);
                    playerPaintBrush.SetColor(Paints.green);
                    playerPaintBottle.SetColor(Paints.green);
                    break;

                case "Green":
                    currentSelectedColour = "Yellow";
                    currentSelectedColourClass = Paints.yellow;

                    _updateUI.ChangePaint(Paints.YELLOW_PAINT, paintQuantity[currentSelectedColour]);
                    playerPaintBrush.SetColor(Paints.yellow);
                    playerPaintBottle.SetColor(Paints.yellow);
                    break;

                case "Yellow":
                    currentSelectedColour = "Blue";
                    currentSelectedColourClass = Paints.blue;

                    _updateUI.ChangePaint(Paints.BLUE_PAINT, paintQuantity[currentSelectedColour]);
                    playerPaintBrush.SetColor(Paints.blue);
                    playerPaintBottle.SetColor(Paints.blue);
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
}