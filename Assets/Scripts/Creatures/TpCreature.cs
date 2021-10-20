using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class TpCreature : MonoBehaviour
{
    // Start is called before the first frame update
    // Start is called before the first frame update
    private LevelManager _levelManager;
    private Material _material;
    private Material _material2;
    private Color _originalColour;
    private Color _originalColour2;

    public bool useMouseClick;

    // For clicking
    public GameObject player;
    private Player _playerx;
    public GameObject tp_creature2;
    public TpCreature[] tp_creaturesx;
    public bool _isPainted;
    private Color _tpCreatureColor = Color.blue;

    public string paintColour1;
    public string paintColour2;
    public int paintQuantity1;
    public int paintQuantity2;

    private UpdateUI _updateUI;
    private bool _isMouseClicked;

    private bool _isMouseOver;
    void Start()
    {
        _levelManager = FindObjectOfType<LevelManager>();
        _playerx = FindObjectOfType<Player>();
        _updateUI = FindObjectOfType<UpdateUI>();
        player = GameObject.FindWithTag("Player");
        tp_creature2 = GameObject.FindWithTag("TpCreature");
        tp_creaturesx  = FindObjectsOfType<TpCreature>();
        if (gameObject.CompareTag("TpCreature"))
        {
            tp_creature2 = GameObject.FindWithTag("TpCreature2");         
        }
        _material = GetComponentInChildren<Renderer>().material;
        _material2 = tp_creature2.GetComponentInChildren<Renderer>().material;
        _originalColour = _material.color;
        _originalColour2 = _material2.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPainted)
        {
            //already painted
            return;
        }
        if (_isMouseOver &&
            SpecialCreatureUtil.ActivateSpecialCreature(
                _isPainted,
                _isMouseOver,
                Input.GetButtonDown("Fire1"),
                player.transform.position,
                transform.position,
                _levelManager,
                paintColour1,
                paintColour2,
                paintQuantity1,
                paintQuantity2,
                _material,
                _tpCreatureColor))
        {
            _originalColour = _material.color;
            _isPainted = true;
            // color other creature:
            _material2.color = _tpCreatureColor;
            _originalColour2 = _material2.color;
            for (int i = 0; i < tp_creaturesx.Length; i++)
            {
                tp_creaturesx[i]._isPainted = true;
            }

        }
    }
    private void OnMouseOver()
    {
        if (useMouseClick && !_isPainted)
        {
            _updateUI.SetInfoText("Needs: " + paintQuantity1 + " " + paintColour1 +
                                  " " + paintQuantity2 + " " + paintColour2);
            _material.color = new Color(0.98f, 1f, 0.45f);
            _isMouseOver = true;
        }
    }

    private void OnMouseExit()
    {
        if (_isPainted)
        {
            return;
        }
        if (useMouseClick)
        {
            _updateUI.SetInfoText("");
            _material.color = _originalColour;
            _isMouseOver = false;
        }
    }
    
    void OnMouseDown()
    {
        if (_isPainted)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < 3)
            {
                
                player.transform.position = tp_creature2.transform.position + Vector3.right ;
                _playerx.UpdateTargetLocation(tp_creature2.transform.position + new Vector3(1.5f, 0f, 0f) );
                // Destroy(gameObject);
            } 
        }
    }
    
}
