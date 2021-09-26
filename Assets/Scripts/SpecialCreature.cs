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
        _material = GetComponent<Renderer>().material;
        _originalColour = _material.color;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            if (_levelManager.GetCurrentlySelectedPaint() == "Red")
            {
                if (_levelManager.HasEnoughPaint())
                {
                    _material.color = Color.blue;
                    _originalColour = _material.color;

                }
            }
        }
    }
}