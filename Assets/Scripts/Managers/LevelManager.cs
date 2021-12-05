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
    [SerializeField] private Paints initialPaintSelected = Paints.Yellow;
    [SerializeField] private int initialYellowPaint = 0;
    [SerializeField] private int initialRedPaint = 0;
    [SerializeField] private int initialGreenPaint = 0;
    [SerializeField] private int initialBluePaint = 0;

    // Level toggles
    [Header("Dev Options")]
    public bool devMode;
    public bool freezePlayer;

    // Player paint info
    private Paints currentSelectedPaint;
    private Color currentSelectedColourClass;
    private Dictionary<Paints, int> paintQuantity;

    private SoundManager _colourChangeSoundManager = new SoundManager();

    // UI info
    private UpdateUI _updateUI;
    private bool _isExitActive;
    private bool _isPanning;

    // Restart/Load checkpoint capabilities
    public static Dictionary<string, dynamic> checkpointInfo;
    public static List<Vector3> pastCheckPoints;
    private RestartDontDeleteManager _restartDontDestroyManager;

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

        // Initialize player paint quantity
        paintQuantity = new Dictionary<Paints, int>();
        paintQuantity.Add(Paints.Yellow, initialYellowPaint); // Raises Platform
        paintQuantity.Add(Paints.Red, initialRedPaint);       // Drops Platform
        paintQuantity.Add(Paints.Green, initialGreenPaint);   // Growing Platform
        paintQuantity.Add(Paints.Blue, initialBluePaint);     // Freezes Platform

        // If dev mode, give super powers!
        if (devMode)
        {
            paintQuantity[Paints.Yellow] = 100;
            paintQuantity[Paints.Red] = 100;
            paintQuantity[Paints.Green] = 100;
            paintQuantity[Paints.Blue] = 100;
        }

        // Initialize UI
        _updateUI = FindObjectOfType<UpdateUI>();
        _updateUI.InitPaintInfoText(paintQuantity[Paints.Yellow], paintQuantity[Paints.Red],
                                    paintQuantity[Paints.Blue], paintQuantity[Paints.Green]);

        // Update Player and UI based on initialPaintSelected
        ChangePaint(initialPaintSelected, false);

        // Audio handling
        _colourChangeSoundManager.SetAudioSources(GetComponents<AudioSource>());
        
        // Keep track whether a level was restarted vs started for the first time.
        _restartDontDestroyManager = FindObjectOfType<RestartDontDeleteManager>();
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

    public void ChangePaint(Paints paint)
    {
        ChangePaint(paint, true);
    }

    public void ChangePaint(Paints paint, bool playEffects)
    {
        if (playEffects)
        {
            _colourChangeSoundManager.PlayRandom();
        }
        
        switch (paint)
        {
            case Paints.Yellow:
                currentSelectedPaint = Paints.Yellow;
                currentSelectedColourClass = GameConstants.Yellow;
                break;

            case Paints.Red:
                currentSelectedPaint = Paints.Red;
                currentSelectedColourClass = GameConstants.Red;
                break;

            case Paints.Green:
                currentSelectedPaint = Paints.Green;
                currentSelectedColourClass = GameConstants.Green;
                break;

            case Paints.Blue:
                currentSelectedPaint = Paints.Blue;
                currentSelectedColourClass = GameConstants.Blue;
                break;
        }

        _updateUI.ChangePaint(currentSelectedPaint, paintQuantity[currentSelectedPaint]);
        playerPaintBrush.SetColor(currentSelectedColourClass, paintQuantity[currentSelectedPaint]);
        playerPaintBottle.SetColor(currentSelectedColourClass, paintQuantity[currentSelectedPaint], playEffects);
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
            SceneLoader.RestartLevel();
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

            // Reset block attributes
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
            paintQuantity[Paints.Blue] = ObjectStorage.paintStates[0];
            paintQuantity[Paints.Green] = ObjectStorage.paintStates[1];
            paintQuantity[Paints.Red] = ObjectStorage.paintStates[2];
            paintQuantity[Paints.Yellow] = ObjectStorage.paintStates[3];
            _updateUI.SetPaint(GetPaintQuantity(GetCurrentlySelectedPaint()));
            _updateUI.InitPaintInfoText(paintQuantity[Paints.Yellow], paintQuantity[Paints.Red],
                                        paintQuantity[Paints.Blue], paintQuantity[Paints.Green]);

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
        _updateUI.EnablePanningUI(isPanning);
        _isPanning = isPanning;
    }

    public bool IsPanning()
    {
        return _isPanning;
    }

    public void IncreasePaint(Paints paintType, int quantity)
    {
        int amount = quantity;
        paintQuantity[paintType] += amount;

        if (currentSelectedPaint == paintType)
        {
            _updateUI.SetPaint(paintQuantity[paintType]);
        }
        _updateUI.UpdatePaintInfoText(paintType, paintQuantity[paintType]);

        playerPaintBrush.SetColor(currentSelectedColourClass, paintQuantity[paintType]);
        playerPaintBottle.SetColor(currentSelectedColourClass, paintQuantity[paintType]);

        MoveRedo lastCommand = redoCommandHandler.LatestCommand() as MoveRedo;
        if (lastCommand)
        {
            lastCommand.RecordPaintSpent(paintType, -quantity);
        }
    }

    public void DecreasePaint(Paints paintType, int quantity)
    {
        int amount = quantity;
        paintQuantity[paintType] -= amount;

        if (currentSelectedPaint == paintType)
        {
            _updateUI.SetPaint(paintQuantity[paintType]);
        }
        _updateUI.UpdatePaintInfoText(paintType, paintQuantity[paintType]);

        playerPaintBrush.SetColor(currentSelectedColourClass, paintQuantity[paintType]);
        playerPaintBottle.SetColor(currentSelectedColourClass, paintQuantity[paintType]);

        MoveRedo lastCommand = redoCommandHandler.LatestCommand() as MoveRedo;
        if (lastCommand)
        {
            lastCommand.RecordPaintSpent(paintType, quantity);
        }

        StopCoroutine("CheckPaintQuantity"); // Stop existing coroutine.
        StartCoroutine("CheckPaintQuantity");
    }

    public void AddPaintInfoToStorage()
    {
        ObjectStorage.paintStates.Add(paintQuantity[Paints.Blue]);
        ObjectStorage.paintStates.Add(paintQuantity[Paints.Green]);
        ObjectStorage.paintStates.Add(paintQuantity[Paints.Red]);
        ObjectStorage.paintStates.Add(paintQuantity[Paints.Yellow]);
    }

    private void ClearUIInfoText()
    {
        _updateUI.ClearUIInfoText();
    }

    public void DecreaseCurrentSelectedPaint(int amount)
    {
        paintQuantity[currentSelectedPaint] -= amount;
        _updateUI.SetPaint(paintQuantity[currentSelectedPaint]);
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

    public Color GetCurrentlySelectedPaintClass()
    {
        return currentSelectedColourClass;
    }

    public Paints GetCurrentlySelectedPaint()
    {
        return currentSelectedPaint;
    }

    public void SetCurrentlySelectedPaint(Paints paintType)
    {
        currentSelectedPaint = paintType;
    }

    public bool HasEnoughPaint()
    {
        return paintQuantity[currentSelectedPaint] > 0;
    }

    public int GetPaintQuantity()
    {
        Paints paintType = GetCurrentlySelectedPaint();
        return paintQuantity[paintType];
    }

    public int GetPaintQuantity(Paints paintType)
    {
        return paintQuantity[paintType];
    }

    public bool IsExitActive()
    {
        return true;
    }

    public void SetExitActive(bool isActive)
    {
        _isExitActive = isActive;
    }
}