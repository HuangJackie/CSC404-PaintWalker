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


    // For clicking
    public GameObject player;
    private Player _playerx;
    public GameObject tp_creature2;
    public TpCreature[] tp_creaturesx;
    private Color _tpCreatureColor = Color.blue;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (isPainted)
        {
            //already painted
            return;
        }

        if (IsMouseOver &&
            SpecialCreatureUtil.ActivateSpecialCreature(
                isPainted,
                IsMouseOver,
                Input.GetButtonDown("Fire1"),
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
            OriginalColour = Material.color;
            isPainted = true;
            // color other creature:
            _material2.color = _tpCreatureColor;
            _originalColour2 = _material2.color;
            for (int i = 0; i < tp_creaturesx.Length; i++)
            {
                tp_creaturesx[i].isPainted = true;
            }
        }
    }

    void OnMouseDown()
    {
        if (isPainted)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < 3)
            {
                Vector3 tpCreaturePosition = tp_creature2.transform.position;
                Vector3 newPlayerPosition = new Vector3(tpCreaturePosition.x + 1, tpCreaturePosition.y + 0.2f,
                    tpCreaturePosition.z);
                player.transform.position = newPlayerPosition;
                _playerx.UpdateTargetLocation(newPlayerPosition);
                // Destroy(gameObject);
            }
        }
    }
}