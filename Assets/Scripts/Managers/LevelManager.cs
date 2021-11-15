using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Microsoft.CSharp;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    //private static LevelManager instance;
    private Dictionary<String, int> paintQuantity;
    private String currentSelectedColour;
    private Color currentSelectedColourClass;
    private bool _isColourSwitched;
    public bool dev_mode;
    public bool freeze_player;

    public RedoCommandHandler redoCommandHandler = new RedoCommandHandler();
    public Player player_script;
    public GameObject player;
    private PaintBrush playerPaintBrush;
    private PaintBottle playerPaintBottle;
    public int init_yellow;
    public int init_red;
    public int init_green;
    public int init_blue;

    //attributes used for restart function
    public static Dictionary<string, dynamic> checkpointInfo;
    public static List<Vector3> pastCheckPoints;

    private UpdateUI _updateUI;
    private bool _isExitActive;
    private bool _isPanning;

    private SoundManager _colourChangeSoundManager = new SoundManager();

    private Queue<Func<IEnumerator>> actionQueue = new Queue<Func<IEnumerator>>();

    //private void Awake()
    //{
    //    if (instance == null)
    //    {
    //        instance = this;
    //        DontDestroyOnLoad(instance);
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //    }
    //}

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

        paintQuantity = new Dictionary<String, int>();
        paintQuantity.Add("Blue", init_blue); // Freezes Platform
        paintQuantity.Add("Green", init_green); // Growing Platform
        paintQuantity.Add("Red", init_red); // Drops Platform
        paintQuantity.Add("Yellow", init_yellow); // Raises Platform

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
        _updateUI.SetPaint(paintQuantity["Yellow"]);

        

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

    public void RestartAtLastCheckpoint()
    {
        Vector3 checkpointPos = checkpointInfo["checkpointPos"];
        if (checkpointPos == Vector3.zero)
        {
            RestartFunction.Restart();
        }
        else
        {
            // the player height on ground is 1.764744f, if we ever change this parameter, the code here needs to be updated as well
            Vector3 spawn_pos = new Vector3(checkpointPos.x, checkpointPos.y + 1.764744f, checkpointPos.z);
            player_script.UpdateTargetLocation(spawn_pos);
            player.transform.position = spawn_pos;
            player_script.resetMode = true;
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
                    block.transform.position = ObjectStorage.blockStates[i][0];
                    groundScript.isPaintedByBrush = ObjectStorage.blockStates[i][1];
                    groundScript.isPaintedByFeet = ObjectStorage.blockStates[i][2];
                    groundScript.originalColour = ObjectStorage.blockStates[i][3];
                    groundScript.paintedColour = ObjectStorage.blockStates[i][4];
                    block.GetComponentInChildren<Renderer>().material.color = ObjectStorage.blockStates[i][5];
                    block.SetActive(ObjectStorage.blockStates[i][6]);
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
                specialCreature.GetComponentInChildren<Renderer>().material.color = ObjectStorage.specialCreatureStates[i][3];
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