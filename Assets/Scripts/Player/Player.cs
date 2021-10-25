using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Player : MonoBehaviour
{

    public LevelManager LevelManager;

    private float _horizontalMovement;
    private float _verticalMovement;
    private bool _isHorizontalMovementPressed;
    private bool _isVerticalMovementPressed;
    private CapsuleCollider _capsuleCollider;
    private Rigidbody _rigidbody;

    // Rigid Grid Movement
    public float speed;
    private Vector3 _targetLocation;
    private Vector3 _prevLocation;

    private UpdateUI _updateUI;
    private Transform _transform;

    private bool _hasWaitedTurn;

    void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        _capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        _targetLocation = transform.position;
        _prevLocation = _targetLocation;
    }

    void Update()
    {
        if (LevelManager.freeze_player)
        {
            return;
        }

        //Debug.DrawRay(_targetLocation + new Vector3(1, -_capsuleCollider.height / 2, 0),
        //    Vector3.up * _capsuleCollider.height, Color.green);
        _horizontalMovement = Input.GetAxisRaw("Horizontal");
        _verticalMovement = Input.GetAxisRaw("Vertical");
        _isHorizontalMovementPressed = Input.GetButton("Horizontal");
        _isVerticalMovementPressed = Input.GetButton("Vertical");

        if (this.CheckGrounded())
        {
            _targetLocation.y = transform.position.y;
            RigidGridMove();
        }
    }

    private void RigidGridMove()
    {
        Vector3 newPosition = Vector3.MoveTowards(
            transform.position, _targetLocation, speed * Time.deltaTime
        );
        if (Vector3.Distance(newPosition, _targetLocation) <= 0.01f)
        {
            newPosition = _targetLocation;
            SetNewTargetLocation(newPosition);
        }

        transform.position = newPosition;

        if (_isHorizontalMovementPressed || _isVerticalMovementPressed)
        {
            LevelManager.SetIsPanning(false);
            Vector3 movDirection = new Vector3(_horizontalMovement, 0f, _verticalMovement);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, Quaternion.LookRotation(movDirection), 0.5f
            );
        }
    }

    public void UpdateTargetLocation(Vector3 newTargetLocation)
    {
        _targetLocation = newTargetLocation;
    }

    private void SetNewTargetLocation(Vector3 currentTransformPosition)
    {
        string pressedButton = GetCurrentlyPressedDirection();

        if (pressedButton == "")
        {
            return;
        }

        if (!ValidMove(pressedButton, currentTransformPosition))
        {
            return;
        }

        _prevLocation = currentTransformPosition;

        switch (pressedButton)
        {
            case "Up":
                _targetLocation = currentTransformPosition + new Vector3(0, 0, 1);
                break;
            case "Down":
                _targetLocation = currentTransformPosition + new Vector3(0, 0, -1);
                break;
            case "Left":
                _targetLocation = currentTransformPosition + new Vector3(-1, 0, 0);
                break;
            case "Right":
                _targetLocation = currentTransformPosition + new Vector3(1, 0, 0);
                break;
        }
    }

    private bool ValidMove(string pressedButton, Vector3 currentTransformPosition)
    {
        RaycastHit hitInfo;
        LayerMask mask = LayerMask.GetMask("Default");

        switch (pressedButton)
        {
            case "Up":
                if (Physics.Raycast(currentTransformPosition +
                                    new Vector3(0, _capsuleCollider.height / 2, 1), Vector3.down, out hitInfo,
                    _capsuleCollider.height, mask))
                {
                    //Debug.Log("Up top is not empty");
                    return noObstructionAhead(hitInfo);
                }

                if (Physics.Raycast(currentTransformPosition +
                                    new Vector3(0, -_capsuleCollider.height / 2, 1), Vector3.up, out hitInfo,
                    _capsuleCollider.height, mask))
                {
                    return false;
                }

                if (!Physics.Raycast(currentTransformPosition + new Vector3(0, 0, 1), Vector3.down, out hitInfo, 1))
                {
                    //Debug.Log("Up bottom is empty");
                    return false;
                }

                return ValidateFloorMove(hitInfo, Vector3.forward, mask);

            case "Down":
                if (Physics.Raycast(currentTransformPosition +
                                    new Vector3(0, _capsuleCollider.height / 2, -1), Vector3.down, out hitInfo,
                    _capsuleCollider.height, mask))
                {
                    //Debug.Log("down top is not empty");
                    return noObstructionAhead(hitInfo);
                }

                if (Physics.Raycast(currentTransformPosition +
                                    new Vector3(0, -_capsuleCollider.height / 2, -1), Vector3.up, out hitInfo,
                    _capsuleCollider.height, mask))
                {
                    return false;
                }

                if (!Physics.Raycast(currentTransformPosition + new Vector3(0, 0, -1), Vector3.down, out hitInfo, 1))
                {
                    //Debug.Log("down bottom is empty");
                    return false;
                }

                return ValidateFloorMove(hitInfo, Vector3.back, mask);

            case "Left":

                if (Physics.Raycast(currentTransformPosition +
                                    new Vector3(-1, _capsuleCollider.height / 2, 0), Vector3.down, out hitInfo,
                    _capsuleCollider.height, mask))
                {
                    // Debug.Log("left top is not empty");
                    return noObstructionAhead(hitInfo);
                }

                if (Physics.Raycast(currentTransformPosition +
                                    new Vector3(-1, -_capsuleCollider.height / 2, 0), Vector3.up, out hitInfo,
                    _capsuleCollider.height, mask))
                {
                    // Debug.Log("Just left is not empty");
                    return false;
                }

                if (!Physics.Raycast(currentTransformPosition + new Vector3(-1, 0, 0), Vector3.down, out hitInfo, 1))
                {
                    // Debug.Log("left bottom is empty ");
                    return false;
                }

                return ValidateFloorMove(hitInfo, Vector3.left, mask);

            case "Right":
                if (Physics.Raycast(currentTransformPosition +
                                    new Vector3(1, _capsuleCollider.height / 2, 0), Vector3.down, out hitInfo,
                    _capsuleCollider.height, mask))
                {
                    //Debug.Log("right top is not empty");
                    return noObstructionAhead(hitInfo);
                }

                if (Physics.Raycast(currentTransformPosition +
                                    new Vector3(1, -_capsuleCollider.height / 2, 0), Vector3.up, out hitInfo,
                    _capsuleCollider.height, mask))
                {
                    return false;
                }

                if (!Physics.Raycast(currentTransformPosition + new Vector3(1, 0, 0), Vector3.down, out hitInfo, 1))
                {
                    //Debug.Log("right bottom is empty");
                    return false;
                }

                return ValidateFloorMove(hitInfo, Vector3.right, mask);
        }

        return false;
    }

    private bool ValidateFloorMove(RaycastHit hitInfo, Vector3 direction, LayerMask mask)
    {
        //Debug.DrawRay(transform.position, Vector3.Normalize(direction), Color.black, 120f);
        if (Physics.Raycast(transform.position, direction, out var hit, 1f)
            && (!IsBlockInFrontPushable(hit) || IsObjectInFrontSpecialCreature(hit))
        )
        {
            return false;
        }

        // If going to hit a wall, don't move.
        if (hitInfo.transform.position.y > 1 && !hitInfo.collider.CompareTag("PaintRefill"))
        {
            return false;
        }

        Ground ground;
        if (hitInfo.collider.gameObject.TryGetComponent(out ground))
        {
            if (ground.isPaintedByBrush || ground.isPaintedByFeet)
            {
                // Painted surface, can move.
                return true;
            }
            // Try to paint.
            return ground.PaintSurface(false); // If false then the floor was not painted.
        }

        return true;
    }

    private bool CheckGrounded()
    {
        //Debug.DrawRay(transform.position, Vector3.down * (_capsuleCollider.height / 2 + +0.1f), Color.black, 120f);
        return Physics.Raycast(transform.position, Vector3.down, _capsuleCollider.height / 2 + 0.1f);
    }

    private bool IsObjectInFrontSpecialCreature(RaycastHit hit)
    {
        return hit.transform.gameObject.CompareTag("TpCreature")
               || hit.transform.gameObject.CompareTag("TpCreature2")
               || hit.transform.gameObject.CompareTag("RammingCreature")
               || hit.transform.gameObject.CompareTag("HowlingCreature");
    }

    private bool noObstructionAhead(RaycastHit hit)
    {
        return IsBlockInFrontPushable(hit) && !IsObjectInFrontSpecialCreature(hit);
    }

    private bool IsBlockInFrontPushable(RaycastHit hit)
    {
        // Debug.Log("Is ground " + hit.transform.gameObject.CompareTag("Ground"));
        return !hit.transform.gameObject.CompareTag("Ground")
               || (hit.collider.gameObject.TryGetComponent(out Ground ground)
                   && (ground.IsIceBlock() &&
                       ground.CanIceBlockSlide(gameObject))); // There is an ice block that can be moved.
    }

    private string GetCurrentlyPressedDirection()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            return "Up";
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            return "Down";
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            return "Left";
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            return "Right";
        }

        return "";
    }
}