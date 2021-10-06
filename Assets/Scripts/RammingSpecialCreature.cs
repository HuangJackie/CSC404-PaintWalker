using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class RammingSpecialCreature : MonoBehaviour
{
    // Start is called before the first frame update
    private LevelManager _levelManager;
    private Material _material;
    private Color _originalColour;
    private bool is_moving = false;
    public int speed = 3;
    public Transform target;

    public bool useMouseClick;

    // For clicking
    public GameObject player;
    public bool _isPainted;

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
        _material = GetComponentInChildren<Renderer>().material;
        _originalColour = _material.color;
        _updateUI = FindObjectOfType<UpdateUI>();
    }

    void Update()
    {
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
                Color.magenta))
        {
            _originalColour = _material.color;
            _isPainted = true;
            is_moving = true;
        }
    }

    private void FixedUpdate()
    {
        if (is_moving)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
            if (Vector3.Distance(transform.position, target.position) < 0.01f)
            {
                is_moving = false;
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        bool shouldActivateSpecialCreature = !useMouseClick
                                             && collision.gameObject.GetComponent<Collider>().CompareTag("Player")
                                             && _levelManager.GetCurrentlySelectedPaint() == "Blue"
                                             && _levelManager.HasEnoughPaint();
        if (shouldActivateSpecialCreature)
        {
            _material.color = Color.green;
            _originalColour = _material.color;
            _levelManager.DecreaseCurrentSelectedPaint(3);
            is_moving = true;
        }

        if (collision.gameObject.GetComponent<Ground>())
        {
            Destroy(collision.gameObject);
        }
    }

    private void OnMouseOver()
    {
        if (useMouseClick && !_isPainted)
        {
            _updateUI.SetInfoText("Needs: " + paintQuantity1 + " " + paintColour1 + " " +
                                         paintQuantity2 +
                                         " " + paintColour2);
            _material.color = new Color(0.98f, 1f, 0.45f);
            _isMouseOver = true;
        }
    }

    private void OnMouseExit()
    {
        if (useMouseClick)
        {
            _updateUI.SetInfoText("");
            _material.color = _originalColour;
            _isMouseOver = false;
        }
    }
}