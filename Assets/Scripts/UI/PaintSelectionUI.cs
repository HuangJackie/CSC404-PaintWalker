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

    /*
     * A dictionary from a Vector3 hash to the collider object housing the paintable object.
     */
    private Dictionary<String, Collider> _positionToCollider;
    private float _maxXValue; // The largest x coordinate.
    private float _minXValue; // The smallest x coordinate.
    private float _maxZValue; // The largest z coordinate.
    private float _minZValue; // The smallest z coordinate.

    /*
     * A dictionary with 5 elements, one per y-axis values when switching the selection in different y-axis levels.
     */
    private Dictionary<int, Collider> _yAxisToCollider;
    private int _maxYValue; // The largest int to be a key in the dictionary above.
    private int _minYValue; // The smallest int to be a key in the dictionary above.

    private Collider _selectedPaintable;

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
            // _levelManager.SetPaintSelectionUIDisplayed(_isActive);

            if (_isActive)
            {
                DisplayPaintSelectionUI();
            }
            else
            {
                ClosePaintSelectionUI();
            }
        }

        if (_isActive)
        {
            if (_selectedPaintable == null)
            {
                Debug.LogError("Something went wrong could not select the block under the player.");
                _isActive = false;
                // _levelManager.SetPaintSelectionUIDisplayed(_isActive);
                ClosePaintSelectionUI();
                return;
            }

            Vector3 currentPosition = _selectedPaintable.transform.position;

            if (_controllerUtil.GetXAxisPaintSelectAxis(out int xSelect))
            {
                bool successfulUpdate =
                    UpdateNextSelectedPaintable(currentPosition, xSelect, Vector3.left, Vector3.right);
                if (!successfulUpdate)
                {
                    TryToSelectNextPaintable(
                        true,
                        currentPosition.y, xSelect,
                        currentPosition.z,
                        currentPosition.x,
                        _minZValue, _maxZValue,
                        _minXValue, _maxXValue
                    );
                }
            }
            else if (_controllerUtil.GetZAxisPaintSelectAxis(out int zSelect))
            {
                bool successfulUpdate =
                    UpdateNextSelectedPaintable(currentPosition, zSelect, Vector3.back, Vector3.forward);
                if (!successfulUpdate)
                {
                    TryToSelectNextPaintable(
                        false,
                        currentPosition.y, zSelect,
                        currentPosition.z,
                        currentPosition.x,
                        _minZValue, _maxZValue,
                        _minXValue, _maxXValue);
                }
            }
            else if (_controllerUtil.GetYAxisPaintSelectAxis(out int ySelect))
            {
                bool successfulUpdate = UpdateNextSelectedPaintable(currentPosition, ySelect, Vector3.up, Vector3.down);
                if (!successfulUpdate)
                {
                    TryToSelectNextPaintableOnYAxis(currentPosition, ySelect);
                }
            }

            if (_controllerUtil.GetPaintButtonDown())
            {
                if (_selectedPaintable.TryGetComponent(out Paintable paintable))
                {
                    paintable.Paint(true);
                }
            }

            if (_controllerUtil.GetInteractButtonDown())
            {
                if (_selectedPaintable.TryGetComponent(out TpCreature teleportCreature))
                {
                    teleportCreature.Interact();
                }
            }
        }
    }

    private void TryToSelectNextPaintable(
        bool flip,
        float yPos,
        float buttonDirection, // This is either -1 or 1 depending on which direction the joystick is toggled.
        float currAxisPos1, float currAxisPos2,
        float minAxisPos1, float maxAxisPos1,
        float minAxisPos2, float maxAxisPos2
    )
    {
        if (flip)
        {
            (currAxisPos1, currAxisPos2) = (currAxisPos2, currAxisPos1);
            (minAxisPos1, minAxisPos2) = (minAxisPos2, minAxisPos1);
            (maxAxisPos1, maxAxisPos2) = (maxAxisPos2, maxAxisPos1);
        }

        if (buttonDirection == 0)
        {
            return;
        }

        float startingPos1 = currAxisPos1;
        float startingPos2 = currAxisPos2;

        int direction = buttonDirection > 0 ? 1 : -1;
        float nextPos1 = GetNextPos(direction, startingPos1, minAxisPos1, maxAxisPos1);
        float nextPos2 = nextPos1 == (buttonDirection > 0 ? minAxisPos1 : maxAxisPos1)
            ? GetNextPos(-direction, startingPos2, minAxisPos2, maxAxisPos2)
            : startingPos2;


        print("minAxisPos1,  maxAxisPos1: " + minAxisPos1 + " to " + maxAxisPos1);
        print("minAxisPos2,  maxAxisPos2: " + minAxisPos2 + " to " + maxAxisPos2);
        int infiniteLoopCheck = 0;
        while (infiniteLoopCheck < 50 && (startingPos1 != nextPos1 || startingPos2 != nextPos2))
        {
            print("startingPos1,  startingPos2 " + startingPos1 + ", " + startingPos2);
            print("nextPos1,  nextPos2 " + nextPos1 + ", " + nextPos2);

            infiniteLoopCheck++;

            Vector3 nextPosition = flip
                ? new Vector3(nextPos1, yPos, nextPos2)
                : new Vector3(nextPos2, yPos, nextPos1);
            if (_positionToCollider.TryGetValue(nextPosition.ToString(), out Collider nextPaintable))
            {
                UpdateHighlightForNextSelected(nextPaintable);
                return;
            }

            nextPos1 = GetNextPos(direction, nextPos1, minAxisPos1, maxAxisPos1);
            if (nextPos1 == (buttonDirection > 0 ? minAxisPos1 : maxAxisPos1))
            {
                nextPos2 = GetNextPos(-direction, nextPos2, minAxisPos2, maxAxisPos2);
            }
        }

        if (infiniteLoopCheck >= 50)
        {
            Debug.LogError("Infinite loop");
        }
    }

    private float GetNextPos(int direction, float currPosition, float minValue, float maxValue)
    {
        currPosition += direction;
        return currPosition > maxValue ? minValue : (currPosition < minValue ? maxValue : currPosition);
    }

    private void TryToSelectNextPaintableOnYAxis(Vector3 currentPosition, float ySelect)
    {
        if (ySelect == 0)
        {
            return;
        }

        // Set the cursor to any paintable object on the next level.
        int currentYLevel = (int) currentPosition.y;
        while (_minYValue <= currentYLevel && currentYLevel <= _maxYValue)
        {
            currentYLevel += ySelect > 0 ? 1 : -1;
            if (_yAxisToCollider.TryGetValue(currentYLevel, out Collider nextPaintable))
            {
                UpdateHighlightForNextSelected(nextPaintable);
                return;
            }
        }
    }

    private bool UpdateNextSelectedPaintable(Vector3 currentPosition, float axisButtonPress, Vector3 positiveUpdate,
        Vector3 negativeUpdate)
    {
        if (axisButtonPress < 0)
        {
            Vector3 nextPosition = currentPosition + positiveUpdate;
            if (_positionToCollider.TryGetValue(nextPosition.ToString(), out Collider nextPaintable))
            {
                UpdateHighlightForNextSelected(nextPaintable);
                return true;
            }
        }
        else if (axisButtonPress > 0)
        {
            Vector3 nextPosition = currentPosition + negativeUpdate;
            if (_positionToCollider.TryGetValue(nextPosition.ToString(), out Collider nextPaintable))
            {
                UpdateHighlightForNextSelected(nextPaintable);
                return true;
            }
        }

        return false;
    }

    private void UpdateHighlightForNextSelected(Collider nextPaintable)
    {
        if (_selectedPaintable.TryGetComponent(out Interactable interactable))
        {
            interactable.UndoHighlight();
            interactable.HighlightForPaintSelectionUI();
            _selectedPaintable = nextPaintable;
            if (_selectedPaintable.TryGetComponent(out interactable))
            {
                interactable.UndoHighlight();
                interactable.HighlightForHoverover();
            }
        }
    }

    /*
     * Initializes the Paint Selection UI. Is called every time the UI is turned on.
     */
    public void DisplayPaintSelectionUI()
    {
        // Compile a dictionary of all blocks within a radius of 2 blocks.
        Collider[] colliders = Physics.OverlapSphere(_player.gameObject.transform.position, 2);
        _positionToCollider = new Dictionary<String, Collider>();
        _yAxisToCollider = new Dictionary<int, Collider>();
        _minYValue = 1000;
        _maxYValue = -1000;
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Paintable _))
            {
                Vector3 colliderPosition = collider.transform.position;
                if (_positionToCollider.ContainsKey(colliderPosition.ToString()))
                {
                    Debug.LogError("There shouldn't be duplicate keys found. Something may have gone wrong.");
                    continue;
                }

                _positionToCollider.Add(colliderPosition.ToString(), collider);

                float xPosition = colliderPosition.x;

                if (_minXValue > xPosition)
                {
                    _minXValue = xPosition;
                }

                if (_maxXValue < xPosition)
                {
                    _maxXValue = xPosition;
                }

                float zPosition = colliderPosition.z;

                if (_minZValue > zPosition)
                {
                    _minZValue = zPosition;
                }

                if (_maxZValue < zPosition)
                {
                    _maxZValue = zPosition;
                }

                int yPosition = (int) colliderPosition.y;
                if (!_yAxisToCollider.ContainsKey(yPosition))
                {
                    if (_minYValue > yPosition)
                    {
                        _minYValue = yPosition;
                    }

                    if (_maxYValue < yPosition)
                    {
                        _maxYValue = yPosition;
                    }

                    _yAxisToCollider.Add(yPosition, collider);
                }

                Interactable collidedInteractable;
                collider.TryGetComponent(out collidedInteractable);
                if (collidedInteractable != null)
                {
                    collidedInteractable.HighlightForPaintSelectionUI();
                }
            }
        }

        // Default the first selected paintable to be the block the player is standing on.
        Vector3 playerPosition = _player.transform.position;
        Vector3 positionBelowPlayer = new Vector3(playerPosition.x, (float) _player.GetYLevel() - 1, playerPosition.z);
        if (!_positionToCollider.TryGetValue(positionBelowPlayer.ToString(), out _selectedPaintable))
        {
            Debug.LogError("The player is not standing on a valid block.");
        }
        else
        {
            Interactable interactable = _selectedPaintable.GetComponent<Interactable>();
            interactable.HighlightForHoverover();
        }
    }

    public void ClosePaintSelectionUI()
    {
        foreach (KeyValuePair<String, Collider> positionColliderPair in _positionToCollider)
        {
            if (positionColliderPair.Value == null)
            {
                continue;
            }

            Interactable collidedInteractable;
            positionColliderPair.Value.TryGetComponent(out collidedInteractable);
            if (collidedInteractable != null)
            {
                collidedInteractable.UndoHighlight();
            }
        }
    }
}