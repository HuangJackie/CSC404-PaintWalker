using System;
using System.Collections;
using DefaultNamespace;
using UnityEngine;
using static GameConstants;

public class PaintingSystem : MonoBehaviour
{
    private Player _player;
    private ControllerUtil _controllerUtil;
    private ChangePerspective _isoCamera;
    private LevelManager _levelManager;

    private Collider _groundBlockBelowPlayer;
    private Collider _currentlySelectedToPaint;
    private bool _isInteractingWithTutorialSign;

    private bool _canInteractWithSelected;

    private GameObject _emptySpaceBlock;
    // private Vector3 _emptySpaceLocation; // To figure out where to place the block.

    /**
     * The currently selected coordinates relative to the player.
     */
    private Vector2 _selectedCoordinatesRelToPlayer;

    private const int YPositionAbovePlayer = 5;

    private void Start()
    {
        _levelManager = FindObjectOfType<LevelManager>();

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
                ResetSelectedObject();
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

        bool xAxisActive = _controllerUtil.GetXAxisPaintSelectAxis(out float xSelect);
        bool zAxisActive = _controllerUtil.GetZAxisPaintSelectAxis(out float zSelect);

        if (Math.Abs(xSelect) > Math.Abs(zSelect))
        {
            zAxisActive = false;
            zSelect = 0;
        }
        else
        {
            xAxisActive = false;
            xSelect = 0;
        }

        // Move painting selection indicator based on the direction
        // the ISO camera is facing
        if (!xAxisActive && !zAxisActive)
        {
            ResetSelectedObject();
            return;
        }

        switch (_isoCamera.direction)
        {
            case CameraDirection.N:
                if (xAxisActive)
                {
                    BestEffortUpdateCurrentlySelectedBlock(xSelect > 0 ? Vector3.right : Vector3.left);
                }
                else
                {
                    BestEffortUpdateCurrentlySelectedBlock(zSelect > 0 ? Vector3.forward : Vector3.back);
                }

                break;

            case CameraDirection.E:
                if (xAxisActive)
                {
                    BestEffortUpdateCurrentlySelectedBlock(xSelect > 0 ? Vector3.back : Vector3.forward);
                }
                else
                {
                    BestEffortUpdateCurrentlySelectedBlock(zSelect > 0 ? Vector3.right : Vector3.left);
                }

                break;

            case CameraDirection.S:
                if (xAxisActive)
                {
                    BestEffortUpdateCurrentlySelectedBlock(xSelect > 0 ? Vector3.left : Vector3.right);
                }
                else
                {
                    BestEffortUpdateCurrentlySelectedBlock(zSelect > 0 ? Vector3.back : Vector3.forward);
                }

                break;

            case CameraDirection.W:
                if (xAxisActive)
                {
                    BestEffortUpdateCurrentlySelectedBlock(xSelect > 0 ? Vector3.forward : Vector3.back);
                }
                else
                {
                    BestEffortUpdateCurrentlySelectedBlock(zSelect > 0 ? Vector3.left : Vector3.right);
                }

                break;
        }
    }
    private void BestEffortUpdateCurrentlySelectedBlock(Vector3 shift, int numShiftsToRecord = 1)
    {
        if (Math.Abs(shift.x - _selectedCoordinatesRelToPlayer.x) < 0.1
            && Math.Abs(shift.z - _selectedCoordinatesRelToPlayer.y) < 0.1)
        {
            return;
        }

        ResetCurrentlySelectedBlock(shift);

        _canInteractWithSelected = true;
        Vector3 playerPosition = _player.transform.position;
        bool hitTopMostObject = RaycastForTopMostObject(
            new Vector3(playerPosition.x,
                0,
                playerPosition.z),
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

    private void ResetCurrentlySelectedBlock(Vector3 shift)
    {
        if (_selectedCoordinatesRelToPlayer.Equals(new Vector2(shift.x, shift.z)))
        {
            return;
        }

        ResetSelectedObject();
    }

    private void HighlightUnpaintableObject(bool hitTopMostObject, RaycastHit hitInfo)
    {
        if (!hitTopMostObject)
        {
            // Means empty space so show a phantom block here.
            return;
        }

        if (hitInfo.collider.TryGetComponent(out Ground _))
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
        if (_controllerUtil.IsPaintButtonPressed() && !_isInteractingWithTutorialSign)
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

        if (RaycastForTopMostObject(_player.transform.position,Vector3.zero,
                                    out RaycastHit hitInfo) &&
            (hitInfo.collider.gameObject.TryGetComponent(out Ground _)))
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
                Mathf.Floor(playerPosition.y - 0.5f) + 0.5f,
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