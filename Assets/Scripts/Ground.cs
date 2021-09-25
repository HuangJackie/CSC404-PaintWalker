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
    
    public void PaintSurface()
    {
        if (_levelManager.HasEnoughPaint())
        {
            _levelManager.DecreaseCurrentSelectedPaint();
            _isPainted = true;
            string currentlySelectedPaint = _levelManager.GetCurrentlySelectedPaint();
            switch (currentlySelectedPaint)
            {
                case "Black":
                    _material.color = Color.black;
                    _originalColour = _material.color;
                    break;
                case "Special":
                    _material.color = Color.cyan;
                    _originalColour = _material.color;
                    break;
            }
        }
    }
}
