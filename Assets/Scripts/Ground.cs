using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public GameObject player;

    public bool _isPainted;
    private LevelManager _levelManager;
    private Material _material;
    private Color _originalColour;
    
    // Start is called before the first frame update
    void Start()
    {
        _isPainted = false;
        _material = GetComponent<Renderer>().material;
        _originalColour = _material.color;
        _levelManager = FindObjectOfType<LevelManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (!_isPainted && other.gameObject.CompareTag("Player"))
        {
            PaintSurface();
        }
    }
    
    public bool PaintSurface()
    {
        if (_levelManager.HasEnoughPaint())
        {
            _levelManager.DecreaseCurrentSelectedPaint();
            _isPainted = true;
            string currentlySelectedPaint = _levelManager.GetCurrentlySelectedPaint();
            switch (currentlySelectedPaint)
            {
                case "Red":
                    _material.color = Color.red;
                    _originalColour = _material.color;
                    break;
                case "Green":
                    _material.color = Color.green;
                    _originalColour = _material.color;
                    break;
                case "Special":
                    _material.color = Color.cyan;
                    _originalColour = _material.color;
                    break;
            }

            return true;
        }

        return false;
    }
}
