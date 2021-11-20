using System;
using DefaultNamespace;
using UnityEngine;
using static GameConstants;

public class PaintingSystem : MonoBehaviour
{
    private Player _player;
    private ControllerUtil _controllerUtil;
    private ChangePerspective _isoCamera;

    private Collider _groundBlockBelowPlayer;
    private Collider _currentlySelectedToPaint;
    private bool _isInteractingWithTutorialSign;

    private bool _canInteractWithSelected;
    private bool _emptySpaceSelected;
    private bool _emptySpaceJustSelected;

    private GameObject _emptySpaceBlock;
    // private Vector3 _emptySpaceLocation; // To figure out where to place the block.

    /**
     * The currently selected coordinates relative to the player.
     */
    private Vector2 _selectedCoordinatesRelToPlayer;

    private const int YPositionAbovePlayer = 5;

    private void Start()
    {
        var emptySpaceBlockModel = Resources.Load(path: "Blocks/EmptySpaceHighlight");
        _emptySpaceBlock = (GameObject) Instantiate(emptySpaceBlockModel, Vector3.zero,
            Quaternion.identity);
        _emptySpaceBlock.SetActive(false);
        
        _player = FindObjectOfType<Player>();
        _controllerUtil = FindObjectOfType<ControllerUtil>();
        _isoCamera = FindObjectOfType<ChangePerspective>();

        UpdateGroundBlockBelowPlayer();
        SetCurrentlySelectedObject(_groundBlockBelowPlayer, Vector2.zero);
        _selectedCoordinatesRelToPlayer = Vector2.zero;

        _isInteractingWithTutorialSign = false;
        _canInteractWithSelected = true;
    }

////////////////////////////////////////////////////////
// BEGIN: Code that Checks for Controller Input
////////////////////////////////////////////////////////
    private void Update()
    {
        ListenForPaintingCommand();
        ListenForMoveSelectInteractableCommand();
        ListenForInteractingCommand();
    }

    private void ListenForInteractingCommand()
    {
        if (_currentlySelectedToPaint == null)
        {
            return;
        }

        // TODO Change to painting button
        if (_controllerUtil.GetPaintButtonDown())
        {
            // Teleport Creature Interaction
            if (_currentlySelectedToPaint.TryGetComponent(out TpCreature teleportCreature)
                && !teleportCreature.IsRecentlyPainted()
                && teleportCreature.isPainted)
            {
                teleportCreature.Interact();
            }

            // Tutorial Sign Interaction
            if (_currentlySelectedToPaint.TryGetComponent(out TutorialToolTips tutorialSign))
            {
                if (!tutorialSign.IsToolTipOpen())
                {
                    tutorialSign.OpenToolTip();
                    _isInteractingWithTutorialSign = true;
                }
                else
                {
                    tutorialSign.CloseToolTip();
                    _isInteractingWithTutorialSign = false;
                }
            }
        }
    }

    private void ListenForMoveSelectInteractableCommand()
    {
        if (_isInteractingWithTutorialSign)
        {
            return;
        }

        bool xAxisActive = _controllerUtil.GetXAxisPaintSelectAxis(out int xSelect);
        bool zAxisActive = _controllerUtil.GetZAxisPaintSelectAxis(out int zSelect);

        // Move painting selection indicator based on the direction
        // the ISO camera is facing
        if (xAxisActive || zAxisActive)
        {
            switch (_isoCamera.direction)
            {
                case CameraDirection.N:
                    if (xAxisActive && IsWithinRange(_selectedCoordinatesRelToPlayer.x + xSelect, 2))
                    {
                        BestEffortUpdateCurrentlySelectedBlock(xSelect == 1 ? Vector3.right : Vector3.left, 1);
                    }
                    else if (IsWithinRange(_selectedCoordinatesRelToPlayer.y + zSelect, 2))
                    {
                        BestEffortUpdateCurrentlySelectedBlock(zSelect == 1 ? Vector3.forward : Vector3.back, 1);
                    }
                    break;

                case CameraDirection.E:
                    if (xAxisActive && IsWithinRange(_selectedCoordinatesRelToPlayer.y - xSelect, 2))
                    {
                        BestEffortUpdateCurrentlySelectedBlock(xSelect == 1 ? Vector3.back : Vector3.forward, 1);
                    }
                    else if (IsWithinRange(_selectedCoordinatesRelToPlayer.x + zSelect, 2))
                    {
                        BestEffortUpdateCurrentlySelectedBlock(zSelect == 1 ? Vector3.right : Vector3.left, 1);
                    }
                    break;

                case CameraDirection.S:
                    if (xAxisActive && IsWithinRange(_selectedCoordinatesRelToPlayer.x - xSelect, 2))
                    {
                        BestEffortUpdateCurrentlySelectedBlock(xSelect == 1 ? Vector3.left : Vector3.right, 1);
                    }
                    else if (IsWithinRange(_selectedCoordinatesRelToPlayer.y - zSelect, 2))
                    {
                        BestEffortUpdateCurrentlySelectedBlock(zSelect == 1 ? Vector3.back : Vector3.forward, 1);
                    }
                    break;

                case CameraDirection.W:
                    if (xAxisActive && IsWithinRange(_selectedCoordinatesRelToPlayer.y + xSelect, 2))
                    {
                        BestEffortUpdateCurrentlySelectedBlock(xSelect == 1 ? Vector3.forward : Vector3.back, 1);
                    }
                    else if (IsWithinRange(_selectedCoordinatesRelToPlayer.x - zSelect, 2))
                    {
                        BestEffortUpdateCurrentlySelectedBlock(zSelect == 1 ? Vector3.left : Vector3.right, 1);
                    }
                    break;
            }
        }
    }

    /**
     * TODO make this into a util function. It currently duplicates the one in ColorWheelQuadrant.
     */
    private bool IsWithinRange(float toCompare, float delta)
    {
        return -delta <= toCompare && toCompare <= delta;
    }

    private void BestEffortUpdateCurrentlySelectedBlock(Vector3 shift, int numShiftsToRecord)
    {
        _canInteractWithSelected = true;
        _emptySpaceJustSelected = false;
        Vector3 playerPosition = _player.transform.position;
        bool hitTopMostObject = RaycastForTopMostObject(
            new Vector3(playerPosition.x + _selectedCoordinatesRelToPlayer.x,
                0,
                playerPosition.z + _selectedCoordinatesRelToPlayer.y),
            shift * numShiftsToRecord,
            out RaycastHit hitInfo);
        if (!hitTopMostObject || !IsSelectableBlock(hitInfo))
        {
            HighlightUnpaintableObject(hitTopMostObject, hitInfo);
        }

        Vector2 newShift = new Vector2(shift.x, shift.z) * numShiftsToRecord;
        if (hitTopMostObject)
        {
            SetCurrentlySelectedObject(hitInfo.collider, newShift);
        }
        else
        {
            SetCurrentlySelectedObject(null, newShift);
        }
    }

    private void HighlightUnpaintableObject(bool hitTopMostObject, RaycastHit hitInfo)
    {
        if (!hitTopMostObject)
        {
            // Means empty space so show a phantom block here.
            _emptySpaceSelected = true;
            _emptySpaceJustSelected = true;
            return;
        }

        if (hitInfo.collider.TryGetComponent(out Ground ground))
        {
            // Means it's an unpaintable ground object.
            _canInteractWithSelected = false;
        }
    }

    private bool IsSelectableBlock(RaycastHit hitInfo)
    {
        TutorialToolTips tutorialSign = null;
        if (!hitInfo.collider.TryGetComponent(out Paintable collidedPaintable) &&
            !hitInfo.collider.TryGetComponent(out tutorialSign))
        {
            return false;
        }

        return (collidedPaintable != null && collidedPaintable.IsPaintable()) || tutorialSign != null;
    }

    private void ListenForPaintingCommand()
    {
        if (_controllerUtil.GetPaintButtonDown() && !_isInteractingWithTutorialSign)
        {
            Paintable paintable = GetCurrentlySelectedComponent<Paintable>();
            if (paintable == null)
            {
                return;
            }

            paintable.Paint(true);
            // Re-highlight new model.
            SetCurrentlySelectedObject(_currentlySelectedToPaint, Vector2.zero);
        }
    }

////////////////////////////////////////////////////////
// END: Code that Checks for Controller Input
////////////////////////////////////////////////////////

    /**
     * Moves the selected block back to the block under the player.
     */
    public void ResetSelectedObject()
    {
        UnhighlightSelectedInteractable();
        UpdateGroundBlockBelowPlayer();
        _currentlySelectedToPaint = _groundBlockBelowPlayer;
        HighlightSelectedInteractable();
        _selectedCoordinatesRelToPlayer = Vector2.zero;
    }

    private void SetCurrentlySelectedObject(Collider newSelectedObject, Vector2 newShift)
    {
        UnhighlightSelectedInteractable();

        _currentlySelectedToPaint = newSelectedObject;
        _selectedCoordinatesRelToPlayer += newShift;

        HighlightSelectedInteractable();
    }

    private void UpdateGroundBlockBelowPlayer()
    {
        if (_player == null)
        {
            return;
        }


        if (RaycastForTopMostObject(_player.transform.position, Vector3.zero, out RaycastHit hitInfo)
            && (hitInfo.collider.gameObject.TryGetComponent(out Ground ground)))
        {
            _groundBlockBelowPlayer = hitInfo.collider;
        }
        else
        {
            LogPaintableObjectNotFoundError();
        }
    }

    private bool RaycastForTopMostObject(Vector3 currentPosition, Vector3 shift, out RaycastHit hitInfo)
    {
        Vector3 positionAbovePlayer = currentPosition + Vector3.up * YPositionAbovePlayer;
        return Physics.Raycast(positionAbovePlayer + shift, Vector3.down, out hitInfo, Mathf.Infinity,
            ~LayerMask.GetMask("Player", "PaintCan"));
    }

    private void HighlightSelectedInteractable()
    {
        if (_currentlySelectedToPaint == null)
        {
            DisplayEmptySpaceBlock();
        }

        Interactable collidedInteractable = GetCurrentlySelectedComponent<Interactable>();
        if (collidedInteractable == null)
        {
            return;
        }

        if (_canInteractWithSelected)
        {
            collidedInteractable.HighlightForPaintSelectionUI();
        }
        else
        {
            collidedInteractable.HighlightForPaintSelectionUIUninteractable();
        }
    }

    private void DisplayEmptySpaceBlock()
    {
        if (_emptySpaceBlock == null)
        {
            return;
        }
        
        Vector3 playerPosition = _player.transform.position;
        _emptySpaceBlock.transform.position =
            new Vector3(playerPosition.x + _selectedCoordinatesRelToPlayer.x,
                Mathf.Floor(playerPosition.y - 1.5f) + 0.5f,
                playerPosition.z + _selectedCoordinatesRelToPlayer.y);
        _emptySpaceBlock.SetActive(true);
    }

    private void UnhighlightSelectedInteractable()
    {
        RemoveEmptySpaceBlock();
        Interactable collidedInteractable = GetCurrentlySelectedComponent<Interactable>();
        if (collidedInteractable != null)
        {
            collidedInteractable.UndoHighlight();
        }
    }

    private void RemoveEmptySpaceBlock()
    {
        if (_emptySpaceBlock == null)
        {
            return;
        }

        _emptySpaceBlock.SetActive(false);
    }

    /**
     * May return default.
     */
    private T GetCurrentlySelectedComponent<T>()
    {
        if (_currentlySelectedToPaint == null)
        {
            return default;
        }

        _currentlySelectedToPaint.TryGetComponent(out T collidedInteractable);
        return collidedInteractable;
    }

    private void LogPaintableObjectNotFoundError()
    {
        Debug.LogError("ERROR: Could not find the ground block below the player.");
    }
}