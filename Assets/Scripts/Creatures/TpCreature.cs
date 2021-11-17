using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class TpCreature : SpecialCreature
{
    // Start is called before the first frame update
    // Start is called before the first frame update
    private LevelManager _levelManager;
    private Material _material2;
    private Color _originalColour2;


    // For painting
    public CameraRotation cameraPanningRevertTarget;
    public GameObject player;
    private ControllerUtil _controllerUtil;
    private Player _playerx;
    public GameObject tp_creature2;
    public TpCreature[] tp_creaturesx;
    private Color _tpCreatureColor = Color.blue;
    
    // To not interact right after painting.
    private bool _isRecentlyPainted;
    
    new void Start()
    {
        base.Start();
        _levelManager = FindObjectOfType<LevelManager>();
        _playerx = FindObjectOfType<Player>();
        player = GameObject.FindWithTag("Player");
        tp_creature2 = GameObject.FindWithTag("TpCreature");
        tp_creaturesx = FindObjectsOfType<TpCreature>();
        if (gameObject.CompareTag("TpCreature"))
        {
            tp_creature2 = GameObject.FindWithTag("TpCreature2");
        }

        _material2 = tp_creature2.GetComponentInChildren<Renderer>().material;
        _originalColour2 = _material2.color;
        _controllerUtil = FindObjectOfType<ControllerUtil>();
        _isRecentlyPainted = false;
    }

    // Update is called once per frame
    void Update()
    {
        _isRecentlyPainted = false;
    }

    new void OnMouseDown()
    {
        base.OnMouseDown();
        Interact();
    }

    public override bool Paint(bool paintWithBrush)
    {
        if (SpecialCreatureUtil.ActivateSpecialCreature(
                isPainted,
                IsMouseOver || _controllerUtil.GetPaintButtonDown(),
                player.transform.position,
                transform.position,
                _levelManager,
                paintColour1,
                paintColour2,
                paintQuantity1,
                paintQuantity2,
                Material,
                _tpCreatureColor))
        {
            originalColour = Material.color;
            paintedColour = Material.color;
            isPainted = true;
            _isRecentlyPainted = true;
            // color other creature:
            _material2.color = _tpCreatureColor;
            _originalColour2 = _material2.color;
            for (int i = 0; i < tp_creaturesx.Length; i++)
            {
                tp_creaturesx[i].isPainted = true;
            }

            return true;
        }
        return false;
    }

    public void Interact()
    {
        if (isPainted)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < 3)
            {
                Vector3 tpCreaturePosition = tp_creature2.transform.position;
                Vector3 newPlayerPosition = new Vector3(tpCreaturePosition.x + 1, tpCreaturePosition.y,
                    tpCreaturePosition.z);
                float tpCreatureYDiff = tp_creature2.transform.position.y - this.transform.position.y;
                Vector3 newCameraPosition = new Vector3(newPlayerPosition.x, newPlayerPosition.y - tpCreatureYDiff, newPlayerPosition.z);
                MoveRedo GameState = ScriptableObject.CreateInstance("MoveRedo") as MoveRedo;
                GameState.PlayerInit(player.gameObject, cameraPanningRevertTarget, newPlayerPosition - player.transform.position, player.transform.rotation);
                _levelManager.redoCommandHandler.AddCommand(GameState);
                _levelManager.redoCommandHandler.TransitionToNewGameState();
                player.transform.position = newPlayerPosition;
                Camera.main.transform.transform.parent.parent.position = newCameraPosition;
                Camera.main.GetComponent<CameraRotation>()._gameplayPos = newCameraPosition;
                _playerx.UpdateTargetLocation(newCameraPosition);
                // Destroy(gameObject);
            }
        }
    }

    public bool IsRecentlyPainted()
    {
        return _isRecentlyPainted;
    }
}