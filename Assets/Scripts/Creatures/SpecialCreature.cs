using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCreature : MonoBehaviour
{
    // Start is called before the first frame update
    private LevelManager _levelManager;
    private Material _material;
    private Color _originalColour;

    void Start()
    {
        _levelManager = FindObjectOfType<LevelManager>();
        _material = GetComponentInChildren<Renderer>().material;
        _originalColour = _material.color;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<Collider>().CompareTag("Player"))
        {
            if (_levelManager.GetCurrentlySelectedPaint() == "Blue")
            {
                if (_levelManager.HasEnoughPaint())
                {
                    _material.color = Color.blue;
                    _originalColour = _material.color;
                    _levelManager.DecreaseCurrentSelectedPaint(2);
                }
            }
        }
    }
}