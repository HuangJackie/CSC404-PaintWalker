using System;
using DefaultNamespace;
using UnityEngine;

public class PaintingSystem : MonoBehaviour
{
    private Player _player;
    private ControllerUtil _controllerUtil;

    private Collider _groundBlockBelowPlayer;
    private Collider _currentlySelectedToPaint;

    /**
     * The currently selected coordinates relative to the player.
     */
    private Vector2 _selectedCoordinatesRelToPlayer;

    private const int YPositionAbovePlayer = 5;

    private void Start()
    {
        _player = FindObjectOfType<Player>();
        _controllerUtil = FindObjectOfType<ControllerUtil>();

        UpdateGroundBlockBelowPlayer();
        SetCurrentlySelectedObject(_groundBlockBelowPlayer);
        _selectedCoordinatesRelToPlayer = Vector2.zero;
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
        if (_controllerUtil.GetInteractButtonDown())
        {
            if (_currentlySelectedToPaint.TryGetComponent(out TpCreature teleportCreature))
            {
                teleportCreature.Interact();
            }
        }
    }

    private void ListenForMoveSelectInteractableCommand()
    {
        if (_controllerUtil.GetXAxisPaintSelectAxis(out int xSelect)
            && IsWithinRange(_selectedCoordinatesRelToPlayer.x + xSelect, 2))
        {
            BestEffortUpdateCurrentlySelectedBlock(xSelect == 1 ? Vector3.right : Vector3.left, 1);
        }
        else if (_controllerUtil.GetZAxisPaintSelectAxis(out int zSelect)
                 && IsWithinRange(_selectedCoordinatesRelToPlayer.y + zSelect, 2))
        {
            BestEffortUpdateCurrentlySelectedBlock(zSelect == 1 ? Vector3.forward : Vector3.back, 1);
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
        print(numShiftsToRecord);
        if (!RaycastForTopMostObject(_currentlySelectedToPaint.transform.position, shift * numShiftsToRecord,
            out RaycastHit hitInfo))
        {
            if ( shift.x != 0 && IsWithinRange(_selectedCoordinatesRelToPlayer.x + shift.x * numShiftsToRecord, 1)
                || shift.z != 0 && IsWithinRange(_selectedCoordinatesRelToPlayer.y + shift.z * numShiftsToRecord, 1))
            {
                BestEffortUpdateCurrentlySelectedBlock(shift, numShiftsToRecord + 1);
            }

            return;
        }

        SetCurrentlySelectedObject(hitInfo.collider);
        _selectedCoordinatesRelToPlayer += new Vector2(shift.x, shift.z) * numShiftsToRecord;
    }

    private void ListenForPaintingCommand()
    {
        if (_controllerUtil.GetPaintButtonDown())
        {
            Paintable paintable = GetCurrentlySelectedComponent<Paintable>();
            if (paintable == null)
            {
                return;
            }

            paintable.Paint(true);
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
        print("Resetting selected block back to player.");
        UnhighlightSelectedInteractable();
        UpdateGroundBlockBelowPlayer();
        _currentlySelectedToPaint = _groundBlockBelowPlayer;
        HighlightSelectedInteractable();
        _selectedCoordinatesRelToPlayer = Vector2.zero;
    }

    private void SetCurrentlySelectedObject(Collider newSelectedObject)
    {
        if (_currentlySelectedToPaint != null)
        {
            UnhighlightSelectedInteractable();
        }

        _currentlySelectedToPaint = newSelectedObject;
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
        Interactable collidedInteractable = GetCurrentlySelectedComponent<Interactable>();
        if (collidedInteractable == null)
        {
            return;
        }

        collidedInteractable.HighlightForPaintSelectionUI();
    }

    private void UnhighlightSelectedInteractable()
    {
        Interactable collidedInteractable = GetCurrentlySelectedComponent<Interactable>();
        if (collidedInteractable == null)
        {
            return;
        }

        collidedInteractable.UndoHighlight();
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