using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class PaintSelectionUI : MonoBehaviour
{
    private Player _player;
    private ControllerUtil _controllerUtil;
    private LevelManager _levelManager;

    private bool _isActive;
    private Dictionary<Vector3, Collider> _positionToCollider;

    // private class SelectableObject
    // {
    //     private Vector3 position;
    //     private SelectableObject up;
    //     private SelectableObject down;
    //     private SelectableObject left;
    //     private SelectableObject right;
    // }

    private void Start()
    {
        _player = FindObjectOfType<Player>();
        _controllerUtil = FindObjectOfType<ControllerUtil>();
        _levelManager = FindObjectOfType<LevelManager>();
        _isActive = false;
    }

    private void Update()
    {
        if (_controllerUtil.PaintSelectionUIToggled())
        {
            _isActive = !_isActive;
            _levelManager.setCanMove(!_isActive);
            
            if (_isActive)
            {
                DisplayPaintSelectionUI();
            }
            else
            {
                ClosePaintSelectionUI();
            }
        }
    }
    private void DisplayPaintSelectionUI()
    {
        Collider[] colliders = Physics.OverlapSphere(_player.gameObject.transform.position, 2);
        _positionToCollider = new Dictionary<Vector3, Collider>();
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Paintable collidedPaintable))
            {
                _positionToCollider.Add(collider.transform.position, collider);
                Interactable collidedInteractable;
                collider.TryGetComponent(out collidedInteractable);
                if (collidedInteractable != null)
                {
                    collidedInteractable.HighlightForPaintSelectionUI();
                }
            }
        }
    }
    
    private void ClosePaintSelectionUI()
    {
        foreach (var positionColliderPair in _positionToCollider)
        {
            Interactable collidedInteractable;
            positionColliderPair.Value.TryGetComponent(out collidedInteractable);
            if (collidedInteractable != null)
            {
                collidedInteractable.UndoHighlight();
            }
        }
    }
}