using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using static GameConstants;

public class Player : MonoBehaviour
{
    public bool resetMode;
    public Transform cameraWorldAxis;
    public CameraRotation cameraPanningRevertTarget;
    public LevelManager LevelManager;
    public ChangePerspective isoCamera;
    public MoveRedo GameState;

    private float _horizontalMovement;
    private float _verticalMovement;
    private bool _isNotTrackingMovement;
    private bool _isHorizontalMovementPressed;
    private bool _isVerticalMovementPressed;
    private bool _isRotating;
    private Vector3 _previousPosForRedo;
    private Quaternion _previsouRotationForRedo;
    private CapsuleCollider _capsuleCollider;
    private Rigidbody _rigidbody;
    public GameObject _colorWheelHUD;

    // Rigid Grid Movement
    public float speed;
    private Vector3 _moveDirection;
    private Vector3 _targetLocation;
    private Vector3 _prevTargetLocation;
    private Vector3 _curposition;
    private Vector3 _prevPosition;

    private UpdateUI _updateUI;

    private bool _hasWaitedTurn;
    private ControllerUtil _controllerUtil;
    private PaintingSystem _paintingSystem;
    private Animator animator;

    private Dictionary<CameraDirection, PlayerDirection> cameraToPlayerDir;

    private void Awake()
    {
        cameraToPlayerDir = new Dictionary<CameraDirection, PlayerDirection>()
        {
            {CameraDirection.None, PlayerDirection.None},
            {CameraDirection.N, PlayerDirection.Forward},
            {CameraDirection.E, PlayerDirection.Right},
            {CameraDirection.S, PlayerDirection.Backward},
            {CameraDirection.W, PlayerDirection.Left}
        };
    }

    private void Start()
    {
        resetMode = false;
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        _capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        UpdateTargetLocation(transform.position);
        _prevTargetLocation = transform.position;
        _prevPosition = _targetLocation;
        _isNotTrackingMovement = true;
        _controllerUtil = FindObjectOfType<ControllerUtil>();
        _paintingSystem = FindObjectOfType<PaintingSystem>();
        _paintingSystem.ResetSelectedObject();
        animator = gameObject.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        _curposition = transform.position;
        Vector3 distMoved = _curposition - _prevPosition;
        _prevPosition = _curposition;

        if (resetMode)
        {
            distMoved = Vector3.zero;
            resetMode = false;
        }

        if (this.gameObject.transform.position.y < -4)
        {
            LevelManager.RestartAtLastCheckpoint();
        }

        cameraWorldAxis.position = cameraWorldAxis.position + new Vector3(0, distMoved.y, 0);
        cameraPanningRevertTarget._gameplayPos =
            cameraPanningRevertTarget._gameplayPos + new Vector3(0, distMoved.y, 0);

        if (LevelManager.freeze_player)
        {
            animation_update("walk", false);
            return;
        }

        //Debug.DrawRay(_targetLocation + new Vector3(1, -_capsuleCollider.height / 2, 0),
        //    Vector3.up * _capsuleCollider.height, Color.green);

        _horizontalMovement = _controllerUtil.GetHorizontalAxisRaw();
        _verticalMovement = _controllerUtil.GetVerticalAxisRaw();
        _isHorizontalMovementPressed = _horizontalMovement != 0;
        _isVerticalMovementPressed = _verticalMovement != 0;

        if (this.CheckGrounded())
        {
            _targetLocation.y = transform.position.y;
            RigidGridMove();
        }

        // Color Wheel HUD
        if (_controllerUtil.GetColourWheelPressed())
        {
            _colorWheelHUD.SetActive(true);
        }

        if (_controllerUtil.GetColourWheelNotPressed())
        {
            _colorWheelHUD.SetActive(false);
        }

        _prevPosition = _curposition;
    }

    public void CreateCopyOfCurrentState(bool up)
    {
        GameState = ScriptableObject.CreateInstance("MoveRedo") as MoveRedo;
        if (up)
        {
            GameState.PlayerInit(this.gameObject, cameraPanningRevertTarget, Vector3.up, transform.rotation);
        }
        else
        {
            GameState.PlayerInit(this.gameObject, cameraPanningRevertTarget, Vector3.down, transform.rotation);
        }

        LevelManager.redoCommandHandler.AddCommand(GameState);
        LevelManager.redoCommandHandler.TransitionToNewGameState();
    }

    public void animation_update(String type, bool state)
    {
        if (type == "idle")
        {
            animator.SetBool("Idle", true);
        }
        if (type == "walk")
        {
            if (state)
            {
                animator.SetBool("Moving", true);
            }
            else
            {
                animator.SetBool("Moving", false);
            }
        }

        if (type == "push")
        {
            if (state)
            {
                animator.SetBool("Pushing", true);
            }
            else
            {
                animator.SetBool("Pushing", false);
                animator.SetBool("Idle", true);
            }
        }
    }
    private void RigidGridMove()
    {
        // animation_update("idle", true);
        if (_targetLocation != transform.position)
        {
            animation_update("walk", true);
            if (InvalidMove())
            {
                _targetLocation = _prevTargetLocation;
            }
            
            if (_isNotTrackingMovement)
            {
                _previousPosForRedo = transform.position;
                _isNotTrackingMovement = false;
            }
        }

        if (_targetLocation == transform.position && !_isNotTrackingMovement)
        {
            if (!_isHorizontalMovementPressed && !_isVerticalMovementPressed)
            {
                animation_update("walk", false);
                print("stopped moving");
            }
            GameState = ScriptableObject.CreateInstance("MoveRedo") as MoveRedo;
            GameState.PlayerInit(this.gameObject, cameraPanningRevertTarget, _targetLocation - _previousPosForRedo,
                _previsouRotationForRedo);
            LevelManager.redoCommandHandler.AddCommand(GameState);
            LevelManager.redoCommandHandler.TransitionToNewGameState();
            _isNotTrackingMovement = true;

            // To reset the selected object to the block under the player. If removing the redo code above,
            // leave this line here.
            _paintingSystem.ResetSelectedObject();
        }

        Vector3 newPosition = Vector3.MoveTowards(
            transform.position, _targetLocation, speed * Time.deltaTime
        );

        if (_isHorizontalMovementPressed || _isVerticalMovementPressed)
        {
            animation_update("walk", true);
        }

        // The player has reached their movement destination.
        if (Vector3.Distance(newPosition, _targetLocation) <= 0.01f)
        {
            //animator.SetBool("Moving", false);
            newPosition = _targetLocation;
            SetNewTargetLocation(newPosition);
        }

        Vector3 horDistMoved = newPosition - transform.position;
        cameraWorldAxis.position = cameraWorldAxis.position + horDistMoved;
        cameraPanningRevertTarget._gameplayPos = cameraPanningRevertTarget._gameplayPos + horDistMoved;

        transform.position = newPosition;

        if (_isHorizontalMovementPressed || _isVerticalMovementPressed)
        {
            LevelManager.SetIsPanning(false);
            _moveDirection = new Vector3(_horizontalMovement, 0f, _verticalMovement);
            _previsouRotationForRedo = transform.rotation;
            _isRotating = true;
        }

        // Set direction of player character
        Vector3 lookDirection = _moveDirection;
        switch (isoCamera.direction)
        {
            case CameraDirection.N:
                // Already set to _moveDirection, so break
                break;
            case CameraDirection.E:
                lookDirection = new Vector3(_moveDirection.z, _moveDirection.y, -_moveDirection.x);
                break;
            case CameraDirection.S:
                lookDirection = -_moveDirection;
                break;
            case CameraDirection.W:
                lookDirection = new Vector3(-_moveDirection.z, _moveDirection.y, _moveDirection.x);
                break;
        }

        if (lookDirection != Vector3.zero && _isRotating)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation, Quaternion.LookRotation(lookDirection), 0.5f
            );
        }

        if (lookDirection != Vector3.zero &&
            Quaternion.Angle(transform.rotation, Quaternion.LookRotation(lookDirection)) < 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
            _isRotating = false;
        }
    }

    private bool InvalidMove()
    {
        LayerMask mask = LayerMask.GetMask("Default") | LayerMask.GetMask("IceCube");

        return Physics.Raycast(_targetLocation, Vector3.down, out RaycastHit hitInfo, 1, mask) 
               && hitInfo.collider.gameObject.TryGetComponent(out Ground ground)
               && ground.IsMoving();
    }

    public void UpdateTargetLocation(Vector3 newTargetLocation)
    {
        _prevTargetLocation = _targetLocation;
        _targetLocation = newTargetLocation;
    }

    private void SetNewTargetLocation(Vector3 currentTransformPosition)
    {
        PlayerDirection pressedDirection = GetCurrentlyPressedDirection();
        if (pressedDirection == PlayerDirection.None)
        {
            return;
        }

        if (!ValidMove(pressedDirection, currentTransformPosition))
        {
            print("Invalid Player move");
            animator.SetBool("Moving", false);
            return;
        }

        switch (pressedDirection)
        {
            case PlayerDirection.Forward:
                UpdateTargetLocation(currentTransformPosition + new Vector3(0, 0, 1));
                break;
            case PlayerDirection.Backward:
                UpdateTargetLocation(currentTransformPosition + new Vector3(0, 0, -1));
                break;
            case PlayerDirection.Left:
                UpdateTargetLocation(currentTransformPosition + new Vector3(-1, 0, 0));
                break;
            case PlayerDirection.Right:
                UpdateTargetLocation(currentTransformPosition + new Vector3(1, 0, 0));
                break;
        }
    }

    private bool ValidMove(PlayerDirection pressedDirection, Vector3 currentTransformPosition)
    {
        RaycastHit hitInfo;
        RaycastHit ground_hitInfo;
        LayerMask mask = LayerMask.GetMask("Default") | LayerMask.GetMask("IceCube");

        if (Physics.Raycast(currentTransformPosition, Vector3.down, out hitInfo, 1, mask)
            && hitInfo.collider.gameObject.TryGetComponent(out Ground ground))
        {
            print("checking ground infront" + ground.IsMoving());
            if (ground.IsMoving())
            {
                // If the ground is in the middle of moving don't let the player move off of it.
                return false;
            }
        }

        switch (pressedDirection)
        {
            case PlayerDirection.Forward:
                if (!Physics.Raycast(currentTransformPosition + new Vector3(0, 0, 1),
                    Vector3.down, out hitInfo, 1, mask))
                {
                    return false;
                }

                ground_hitInfo = hitInfo;
                if (Physics.Raycast(currentTransformPosition +
                                    new Vector3(0, _capsuleCollider.height / 2, 1), Vector3.down, out hitInfo,
                    _capsuleCollider.height, mask))
                {
                    return noObstructionAhead(hitInfo);
                }

                if (Physics.Raycast(currentTransformPosition +
                                    new Vector3(0, -_capsuleCollider.height / 2, 1), Vector3.up, out hitInfo,
                    _capsuleCollider.height, mask))
                {
                    return false;
                }

                return ValidateFloorMove(ground_hitInfo, Vector3.forward, mask);

            case PlayerDirection.Backward:
                if (!Physics.Raycast(currentTransformPosition + new Vector3(0, 0, -1), Vector3.down, out hitInfo, 1,
                    mask))
                {
                    return false;
                }

                ground_hitInfo = hitInfo;
                if (Physics.Raycast(currentTransformPosition +
                                    new Vector3(0, _capsuleCollider.height / 2, -1), Vector3.down, out hitInfo,
                    _capsuleCollider.height, mask))
                {
                    return noObstructionAhead(hitInfo);
                }

                if (Physics.Raycast(currentTransformPosition +
                                    new Vector3(0, -_capsuleCollider.height / 2, -1), Vector3.up, out hitInfo,
                    _capsuleCollider.height, mask))
                {
                    return false;
                }

                return ValidateFloorMove(ground_hitInfo, Vector3.back, mask);

            case PlayerDirection.Left:
                if (!Physics.Raycast(currentTransformPosition + new Vector3(-1, 0, 0), Vector3.down, out hitInfo, 1,
                    mask))
                {
                    return false;
                }

                ground_hitInfo = hitInfo;
                if (Physics.Raycast(currentTransformPosition +
                                    new Vector3(-1, _capsuleCollider.height / 2, 0), Vector3.down, out hitInfo,
                    _capsuleCollider.height, mask))
                {
                    return noObstructionAhead(hitInfo);
                }

                if (Physics.Raycast(currentTransformPosition +
                                    new Vector3(-1, -_capsuleCollider.height / 2, 0), Vector3.up, out hitInfo,
                    _capsuleCollider.height, mask))
                {
                    return false;
                }

                return ValidateFloorMove(ground_hitInfo, Vector3.left, mask);

            case PlayerDirection.Right:
                if (!Physics.Raycast(currentTransformPosition + new Vector3(1, 0, 0), Vector3.down, out hitInfo, 1,
                    mask))
                {
                    return false;
                }

                ground_hitInfo = hitInfo;
                if (Physics.Raycast(currentTransformPosition +
                                    new Vector3(1, _capsuleCollider.height / 2, 0), Vector3.down, out hitInfo,
                    _capsuleCollider.height, mask))
                {
                    return noObstructionAhead(hitInfo);
                }

                if (Physics.Raycast(currentTransformPosition +
                                    new Vector3(1, -_capsuleCollider.height / 2, 0), Vector3.up, out hitInfo,
                    _capsuleCollider.height, mask))
                {
                    return false;
                }

                return ValidateFloorMove(ground_hitInfo, Vector3.right, mask);
        }

        return false;
    }

    private bool ValidateFloorMove(RaycastHit hitInfo, Vector3 direction, LayerMask mask)
    {
        //Debug.DrawRay(transform.position, Vector3.Normalize(direction), Color.black, 120f);
        if (Physics.Raycast(transform.position, direction, out var hit, 1f) &&
            (!IsBlockInFrontPushable(hit) || IsObjectInFrontSpecialCreature(hit)))
        {
            return false;
        }

        Ground ground;
        if (hitInfo.collider.gameObject.TryGetComponent(out ground))
        {
            print(ground.transform.position.y);
            // If the ground is in the middle of moving don't let the player move on to it.
            if (ground.IsMoving())
            {
                return false;
            }

            if (ground.isPaintedByBrush || ground.isPaintedByFeet || !ground.IsPaintable())
            {
                // Painted surface or not painted in the first place, can move.
                return true;
            }

            // Try to paint.
            return ground.Paint(_controllerUtil.IsPaintButtonPressed()); // If false then the floor was not painted.
        }

        return true;
    }

    private bool CheckGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _capsuleCollider.height * 40 / 2 + 0.1f);
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

    private PlayerDirection GetCurrentlyPressedDirection()
    {
        // Get the current camera direction as N,E,S or W/
        // This will be the forward direction for player
        CameraDirection camForwardDir = isoCamera.direction;

        if (_verticalMovement > 0) // Forward
        {
            return GetCameraToPlayerDir(camForwardDir, 0);
        }
        else if (_verticalMovement < 0) // Backward
        {
            return GetCameraToPlayerDir(camForwardDir, 2);
        }
        else if (_horizontalMovement < 0) // Left
        {
            return GetCameraToPlayerDir(camForwardDir, 3);
        }
        else if (_horizontalMovement > 0) // Right
        {
            return GetCameraToPlayerDir(camForwardDir, 1);
        }

        return PlayerDirection.None;
    }

    // Return the value corresponding to cameraToPlayerDir[camDir + offset]
    // while ensuring the index is not out-of-bounds
    // offset - Must be a positive integer < 4
    private PlayerDirection GetCameraToPlayerDir(CameraDirection camDir, int offset)
    {
        int camDirInt = (int) camDir;

        if (camDirInt + offset > 4)
        {
            int cycleCompletionAmount = 4 - camDirInt;
            int newOffset = offset - cycleCompletionAmount;
            return cameraToPlayerDir[(CameraDirection) newOffset];
        }
        else
        {
            return cameraToPlayerDir[(CameraDirection) (camDirInt + offset)];
        }
    }

    public double GetYLevel()
    {
        return Math.Floor(transform.position.y);
    }
}