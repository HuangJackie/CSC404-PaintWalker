using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using static GameConstants;

public class LevelManager : MonoBehaviour
{
    // Player info
    [Header("Player")]
    public GameObject player;
    public Player playerBehavior;
    private PaintBrush playerPaintBrush;
    private PaintBottle playerPaintBottle;

    // Player initialization for this Level set in inspector
    [Header("Player Initialization")]
    [SerializeField] private int initialYellowPaint = 0;
    [SerializeField] private int initialRedPaint = 0;
    [SerializeField] private int initialGreenPaint = 0;
    [SerializeField] private int initialBluePaint = 0;

    // Level toggles
    [Header("Dev Options")]
    public bool devMode;
    public bool freezePlayer;

    // Player paint info
    private String currentSelectedColour;
    private Color currentSelectedColourClass;
    private Dictionary<String, int> paintQuantity;

    private SoundManager _colourChangeSoundManager = new SoundManager();

    // UI info
    private UpdateUI _updateUI;
    private bool _isExitActive;
    private bool _isPanning;

    // Restart/Load checkpoint capabilities
    public static Dictionary<string, dynamic> checkpointInfo;
    public static List<Vector3> pastCheckPoints;

    // Redo capabilities
    public RedoCommandHandler redoCommandHandler = new RedoCommandHandler();
    private Queue<Func<IEnumerator>> actionQueue = new Queue<Func<IEnumerator>>();

    private void Awake()
    {
        LevelManager.pastCheckPoints = new List<Vector3>();
        checkpointInfo = new Dictionary<string, dynamic>();
        checkpointInfo["checkpointPos"] = Vector3.zero;

        playerPaintBrush = FindObjectOfType<PaintBrush>();
        playerPaintBottle = FindObjectOfType<PaintBottle>();
    }

    void Start()
    {
        StartCoroutine(ManageCoroutines());
        freezePlayer = false;

        paintQuantity = new Dictionary<String, int>();
        paintQuantity.Add("Blue", initialBluePaint);     // Freezes Platform
        paintQuantity.Add("Green", initialGreenPaint);   // Growing Platform
        paintQuantity.Add("Red", initialRedPaint);       // Drops Platform
        paintQuantity.Add("Yellow", initialYellowPaint); // Raises Platform

        if (devMode)
        {
            paintQuantity["Blue"] = 100;
            paintQuantity["Green"] = 100;
            paintQuantity["Red"] = 100;
            paintQuantity["Yellow"] = 100;
        }
        
        currentSelectedColour = "Yellow";
        currentSelectedColourClass = GameConstants.yellow;

        playerPaintBrush.SetColor(currentSelectedColourClass, paintQuantity[currentSelectedColour]);
        playerPaintBottle.SetColor(currentSelectedColourClass, paintQuantity[currentSelectedColour]);

        _updateUI = FindObjectOfType<UpdateUI>();  // Auto-sets yellow to 3/10
        _updateUI.SetPaint(paintQuantity["Yellow"]);
        _updateUI.InitPaintInfoText(paintQuantity["Yellow"], paintQuantity["Red"],
                                    paintQuantity["Blue"], paintQuantity["Green"]);

        _colourChangeSoundManager.SetAudioSources(GetComponents<AudioSource>());
    }

    void Update()
    {
        if (freezePlayer)
        {
            return;
        }
    }

    IEnumerator ManageCoroutines()
    {
        while (true)
        {
            while (actionQueue.Count > 0)
            {
                yield return StartCoroutine(actionQueue.Dequeue()());
            }
            yield return null;
        }
    }

    public void EnqueueAction(Func<IEnumerator> action)
    {
        actionQueue.Enqueue(action);
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
                playerPaintBrush.SetColor(currentSelectedColourClass, paintQuantity[currentSelectedColour]);
                playerPaintBottle.SetColor(currentSelectedColourClass, paintQuantity[currentSelectedColour]);
                break;

            case "Green":
                currentSelectedColour = "Green";
                currentSelectedColourClass = GameConstants.green;

                _updateUI.ChangePaint(GameConstants.GREEN_PAINT, paintQuantity[currentSelectedColour]);
                playerPaintBrush.SetColor(currentSelectedColourClass, paintQuantity[currentSelectedColour]);
                playerPaintBottle.SetColor(currentSelectedColourClass, paintQuantity[currentSelectedColour]);
                break;

            case "Yellow":
                currentSelectedColour = "Yellow";
                currentSelectedColourClass = GameConstants.yellow;

                _updateUI.ChangePaint(GameConstants.YELLOW_PAINT, paintQuantity[currentSelectedColour]);
                playerPaintBrush.SetColor(currentSelectedColourClass, paintQuantity[currentSelectedColour]);
                playerPaintBottle.SetColor(currentSelectedColourClass, paintQuantity[currentSelectedColour]);
                break;

            case "Blue":
                currentSelectedColour = "Blue";
                currentSelectedColourClass = GameConstants.blue;

                _updateUI.ChangePaint(GameConstants.BLUE_PAINT, paintQuantity[currentSelectedColour]);
                playerPaintBrush.SetColor(currentSelectedColourClass, paintQuantity[currentSelectedColour]);
                playerPaintBottle.SetColor(currentSelectedColourClass, paintQuantity[currentSelectedColour]);
                break;
        }
    }

    IEnumerator CheckPaintQuantity()
    {
        // Takes time to collect the paint and replenish
        yield return new WaitForSeconds(0.5f);

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

        if (playerBehavior != null)
        {
            playerBehavior.UpdateTargetLocation(playerBehavior.gameObject.transform.position);
        }
    }

    public void RestartAtLastCheckpoint()
    {
        Vector3 checkpointPos = checkpointInfo["checkpointPos"];
        if (checkpointPos == Vector3.zero)
        {
            RestartFunction.Restart();
        }
        else
        {
            Vector3 playerPos = checkpointInfo["playerPos"];

            // NOTE: The player height on ground is 1.764744f,
            // if we ever change this parameter, the code here needs to be updated as well
            Vector3 spawn_pos = new Vector3(checkpointPos.x, playerPos.y, checkpointPos.z);
            print(spawn_pos);
            playerBehavior.UpdateTargetLocation(spawn_pos);
            player.transform.position = spawn_pos;
            playerBehavior.resetMode = true;
            player.transform.rotation = checkpointInfo["playerRotation"];

            CameraRotation cameraToMove = checkpointInfo["cameraAttributes"];
            cameraToMove.transform.parent.parent.position = spawn_pos;
            if (cameraToMove._wasPanning)
            {
                cameraToMove._gameplayPos = spawn_pos;
                SetIsPanning(false);
            }

            //remove new blocks that got added after the checkpoint
            //int storedBlockNumber = ObjectStorage.blockStates.Count;
            //int currentBlockNumber = ObjectStorage.blockStorage.Count;
            //if (currentBlockNumber > storedBlockNumber)
            //{
            //    ObjectStorage.blockStorage = ObjectStorage.blockStorage[0: storedBlockNumber];
            //}

            //reset block attributes
            for (int i = 0; i < ObjectStorage.blockStorage.Count; i++)
            {
                GameObject block = ObjectStorage.blockStorage[i];
                if (i+1 > ObjectStorage.blockStates.Count)
                {
                    Destroy(block);
                }
                else
                {
                    Ground groundScript = block.GetComponent<Ground>();
                    groundScript.UpdateModel(ObjectStorage.blockStates[i][7]);
                    block.transform.position = ObjectStorage.blockStates[i][0];
                    groundScript.isPaintedByBrush = ObjectStorage.blockStates[i][1];
                    groundScript.isPaintedByFeet = ObjectStorage.blockStates[i][2];
                    groundScript.originalColour = ObjectStorage.blockStates[i][3];
                    groundScript.paintedColour = ObjectStorage.blockStates[i][4];
                    groundScript._paintedColour = ObjectStorage.blockStates[i][9];
                    block.GetComponentInChildren<Renderer>().material.color = ObjectStorage.blockStates[i][5];
                    groundScript.destinationNeutral = ObjectStorage.blockStates[i][8];
                    groundScript._destinationDrop = ObjectStorage.blockStates[i][8] + new Vector3(0, -1, 0);
                    groundScript._destinationRaise = ObjectStorage.blockStates[i][8] - new Vector3(0, -1, 0);
                    groundScript.isWalkedOverHorizontally = ObjectStorage.blockStates[i][10];
                    groundScript.isWalkedOverVertially = ObjectStorage.blockStates[i][11];
                    block.SetActive(ObjectStorage.blockStates[i][6]);
                    groundScript.ReinitializeMaterialColours();
                }
            }

            //reset paintOrb attributes
            for (int i = 0; i < ObjectStorage.paintOrbStorage.Count; i++)
            {
                GameObject paintOrb = ObjectStorage.paintOrbStorage[i];
                paintOrb.transform.position = ObjectStorage.paintOrbStates[i][0];
                paintOrb.SetActive(ObjectStorage.paintOrbStates[i][1]);
            }

            //reset specialCreature attributes
            for (int i = 0; i < ObjectStorage.specialCreatureStorage.Count; i++)
            {
                SpecialCreature specialCreature = ObjectStorage.specialCreatureStorage[i];
                specialCreature.gameObject.transform.position = ObjectStorage.specialCreatureStates[i][0];
                specialCreature.isPainted = ObjectStorage.specialCreatureStates[i][1];
                specialCreature.originalColour = ObjectStorage.specialCreatureStates[i][2];
                specialCreature.paintedColour = ObjectStorage.specialCreatureStates[i][3];
                specialCreature.GetComponentInChildren<Renderer>()
                               .material.color =ObjectStorage.specialCreatureStates[i][3];
                specialCreature.gameObject.SetActive(ObjectStorage.specialCreatureStates[i][4]);
            }

            //reset wall attributes
            for (int i = 0; i < ObjectStorage.wallStorage.Count; i++)
            {
                GameObject wall = ObjectStorage.wallStorage[i];
                wall.transform.position = ObjectStorage.wallStates[i][0];
                wall.GetComponent<MoveWall>().operate = ObjectStorage.wallStates[i][1];
            }

            //reset paint amount
            paintQuantity["Blue"] = ObjectStorage.paintStates[0];
            paintQuantity["Green"] = ObjectStorage.paintStates[1];
            paintQuantity["Red"] = ObjectStorage.paintStates[2];
            paintQuantity["Yellow"] = ObjectStorage.paintStates[3];
            _updateUI.SetPaint(GetPaintQuantity(GetCurrentlySelectedPaint()));
            _updateUI.InitPaintInfoText(paintQuantity["Yellow"], paintQuantity["Red"],
                                        paintQuantity["Blue"], paintQuantity["Green"]);

            //reset foot step FX and Sparkle FX
            for (int i = ObjectStorage.footPrintStates.Count; i < ObjectStorage.footStepStorage.Count; i++)
            {
                GameObject footprint = ObjectStorage.footStepStorage[i];
                Destroy(footprint);
            }

            for (int i = ObjectStorage.sparkleStates.Count; i < ObjectStorage.sparkleStorage.Count; i++)
            {
                GameObject sparkle = ObjectStorage.sparkleStorage[i];
                Destroy(sparkle);
            }
        }
        actionQueue.Clear();
        StopAllCoroutines();
        StartCoroutine(ManageCoroutines());
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
        _updateUI.UpdatePaintInfoText(paintColour, paintQuantity[paintColour]);

        playerPaintBrush.SetColor(currentSelectedColourClass, paintQuantity[paintColour]);
        playerPaintBottle.SetColor(currentSelectedColourClass, paintQuantity[paintColour]);

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
        _updateUI.UpdatePaintInfoText(paintColour, paintQuantity[paintColour]);

        playerPaintBrush.SetColor(currentSelectedColourClass, paintQuantity[paintColour]);
        playerPaintBottle.SetColor(currentSelectedColourClass, paintQuantity[paintColour]);

        MoveRedo lastCommand = redoCommandHandler.LatestCommand() as MoveRedo;
        if (lastCommand)
        {
            lastCommand.RecordPaintSpent(paintColour, quantity);
        }

        StopCoroutine("CheckPaintQuantity"); // Stop existing coroutine.
        StartCoroutine("CheckPaintQuantity");
    }

    public void AddPaintInfoToStorage()
    {
        ObjectStorage.paintStates.Add(paintQuantity["Blue"]);
        ObjectStorage.paintStates.Add(paintQuantity["Green"]);
        ObjectStorage.paintStates.Add(paintQuantity["Red"]);
        ObjectStorage.paintStates.Add(paintQuantity["Yellow"]);
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
        //return _isExitActive;
        return true;
    }

    public void SetExitActive(bool isActive)
    {
        _isExitActive = isActive;
    }
}