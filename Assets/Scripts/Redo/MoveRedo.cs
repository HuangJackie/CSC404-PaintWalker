using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRedo : ScriptableObject, IRedoCommand
{
    public LevelManager leveManager;
    private string colorSpent;
    private int amountSpent;
    private Vector3 direction = Vector3.zero;
    private GameObject player;
    private List<GameObject> objects;
    private List<GameObject> greenBlocksToRevert;
    private List<Vector3> _objectsOriginPosition;
    private List<(bool, bool, Color, Color)> blockMetadata;
    private CameraRotation cameraToMove;
    private Vector3 player_destination;
    private Vector3 originalPlayerPos;
    private Vector3 playerYOffset = Vector3.zero;
    private Quaternion player_rotation;
    public void PlayerInit(GameObject player, CameraRotation cameraToMove, Vector3 player_destination, Quaternion player_rotation)
    {
        this.player = player;
        this.originalPlayerPos = player.transform.position;
        this.player_destination = player_destination;
        this.cameraToMove = cameraToMove;
        this.player_rotation = player_rotation;
    }

    public void ObjectInit(GameObject obj)
    {
        this.objects = new List<GameObject>();
        this.blockMetadata = new List<(bool, bool, Color, Color)>();
        this.objects.Add(obj);
        this._objectsOriginPosition = new List<Vector3>();
        foreach (GameObject i in this.objects)
        {
            this._objectsOriginPosition.Add(i.transform.position);
            if (i.gameObject.CompareTag("Ground"))
            {
                Ground groundScript = i.gameObject.GetComponent<Ground>();
                this.blockMetadata.Add((groundScript.isPaintedByFeet, groundScript.isPaintedByBrush, ((Interactable) groundScript).originalColour, groundScript.originalColour));
            }
        }
    }

    public void Undo()
    {
        leveManager = FindObjectOfType<LevelManager>();
        if (player)
        {
            RevertPlayerAndCameraState();
        }
        if (objects != null)
        {
            Debug.Log("objs redo activated");
            int index = 0;
            int ground_index = 0;
            foreach (GameObject obj in objects)
            {
                if (obj != null)
                {
                    obj.transform.position = _objectsOriginPosition[index];
                    if (obj.gameObject.CompareTag("Ground"))
                    {
                        Ground groundScript = obj.gameObject.GetComponent<Ground>();
                        groundScript.isPaintedByFeet = blockMetadata[ground_index].Item1;
                        groundScript.isPaintedByBrush = blockMetadata[ground_index].Item2;
                        groundScript.originalColour = blockMetadata[ground_index].Item3;
                        if (blockMetadata[ground_index].Item3 == new Color(0.98f, 1f, 0.45f))
                        {
                            obj.gameObject.GetComponentInChildren<Renderer>().material.color = new Color(1.000f, 0.993f, 0.816f);
                        }
                        else
                        {
                            obj.gameObject.GetComponentInChildren<Renderer>().material.color = blockMetadata[ground_index].Item3;
                        }
                        ground_index++;
                    }
                    else if (obj.gameObject.CompareTag("PaintRefill"))
                    {
                        obj.SetActive(true);
                    }
                    index++;
                }
            }
        }
        if (greenBlocksToRevert != null)
        {
            foreach (GameObject obj in greenBlocksToRevert)
            {
                Destroy(obj);
            }
        }
        if (this.colorSpent != null)
        {
            Debug.Log("increasing paint");
            leveManager.IncreasePaint(this.colorSpent, this.amountSpent);
        }
        
    }

    public void RecordPaintSpent(string color, int amount)
    {
        this.amountSpent = amount;
        this.colorSpent = color;
    }

    public void UpdatePlayerY(float y_pos)
    {
        float yOffset = y_pos - originalPlayerPos.y;
        playerYOffset = new Vector3(0, yOffset, 0);
        //camYOffset = 
    }

    public void InjectBlockState(GameObject obj)
    {
        Ground groundScript = obj.gameObject.GetComponent<Ground>();
        if (blockMetadata == null)
        {
            this.blockMetadata = new List<(bool, bool, Color, Color)>();
        }
        if (objects == null)
        {
            this.objects = new List<GameObject>();
        }
        if (this._objectsOriginPosition == null)
        {
            this._objectsOriginPosition = new List<Vector3>();
        }
        objects.Add(obj);
        // Debug.Log(obj.transform.position);
        this._objectsOriginPosition.Add(obj.transform.position);
        this.blockMetadata.Add((groundScript.isPaintedByFeet, groundScript.isPaintedByBrush, ((Interactable) groundScript).originalColour, groundScript.originalColour));
    }

    public void InjectPaintPickup(GameObject obj)
    {
        if (objects == null)
        {
            this.objects = new List<GameObject>();
        }
        if (this._objectsOriginPosition == null)
        {
            this._objectsOriginPosition = new List<Vector3>();
        }
        objects.Add(obj);
        this._objectsOriginPosition.Add(obj.transform.position);
    }

    public void AddGreenBlocksToRevert(GameObject obj)
    {
        if (greenBlocksToRevert == null)
        {
            greenBlocksToRevert = new List<GameObject>();
            greenBlocksToRevert.Add(obj);
        }
        else
        {
            greenBlocksToRevert.Add(obj);
        }
    }

    public void RevertPlayerAndCameraState()
    {
        player.transform.position -= player_destination;
        player.transform.rotation = player_rotation;
        if (cameraToMove._wasPanning)
        {
            cameraToMove.transform.parent.parent.position = cameraToMove._gameplayPos;
            if (player_destination != Vector3.up && player_destination != Vector3.down)
            {
                cameraToMove._gameplayPos -= player_destination;
            }
               
        }
        else if (player_destination != Vector3.up && player_destination != Vector3.down)
        {
            cameraToMove.transform.parent.parent.position -= player_destination;
        }
    }
}
