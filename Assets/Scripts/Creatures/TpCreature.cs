using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using static GameConstants;

public class TpCreature : SpecialCreature
{
    private LevelManager _levelManager;
    private Material _material2;
    private Color _originalColour2;

    // For painting
    public CameraRotation cameraPanningRevertTarget;
    public GameObject player;
    private ControllerUtil _controllerUtil;
    private Player _playerx;
    public GameObject tp_creature2;
    private TpCreature tp_creature2_Object;
    public TpCreature[] tp_creaturesx;
    private Color _tpCreatureColor = Color.blue;
    
    // To not interact right after painting.
    private bool _isRecentlyPainted;

    // Coordinates to TP the player
    public Vector3 teleportPosition;
    new void Start()
    {
        base.Start();
        _levelManager = FindObjectOfType<LevelManager>();
        _playerx = FindObjectOfType<Player>();
        player = GameObject.FindWithTag("Player");
        tp_creature2_Object = tp_creature2.GetComponent<TpCreature>();
        tp_creaturesx = new TpCreature[2];
        tp_creaturesx[0] = this;
        tp_creaturesx[1] = tp_creature2_Object;
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

    private void color_creature()
    {
        frozen_model.SetActive(false);
        coloured_model.SetActive(true);
    }

    public override bool Paint(bool paintWithBrush)
    {
        if (SpecialCreatureUtil.ActivateSpecialCreature(
                isPainted,
                true,
                player.transform.position,
                transform.position,
                _levelManager,
                paintType1,
                paintType2,
                paintQuantity1,
                paintQuantity2,
                Material,
                _tpCreatureColor))
        {
            player.GetComponent<Player>().animation_update("paint", true);
            // originalColour = Material.color;
            // paintedColour = Material.color;
            isPainted = true;
            _isRecentlyPainted = true;
            // color other creature:
            // _material2.color = _tpCreatureColor;
            // _originalColour2 = _material2.color;
            for (int i = 0; i < tp_creaturesx.Length; i++)
            {
                // frozen_model.SetActive(false);
                // coloured_model.SetActive(true);
                tp_creaturesx[i].color_creature();
                tp_creaturesx[i].isPainted = true;

                if (tp_creaturesx[i].TryGetComponent(out Interactable interactable))
                {
                    interactable.paintedColour = Material.color;
                    interactable.ReinitializeMaterialColours();
                }
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
                // Vector3 tpCreaturePosition = tp_creature2_Object.teleportPosition;
                Vector3 newPlayerPosition = tp_creature2_Object.teleportPosition;
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