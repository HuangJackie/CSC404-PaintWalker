using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Player : MonoBehaviour
{
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
    private GameObject _colorWheelHUD;

    // Rigid Grid Movement
    public float speed;
    private Vector3 _moveDirection;
    private Vector3 _targetLocation;
    private Vector3 _curposition;
    private Vector3 _prevPosition;

    private UpdateUI _updateUI;

    private bool _hasWaitedTurn;
    private ControllerUtil _controllerUtil;
    private PaintingSystem _paintingSystem;

    void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        _capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
        _targetLocation = transform.position;
        _prevPosition = _targetLocation;
        _isNotTrackingMovement = true;
        _controllerUtil = FindObjectOfType<ControllerUtil>();
        _paintingSystem = FindObjectOfType<PaintingSystem>();
        _paintingSystem.ResetSelectedObject();
        _colorWheelHUD = GameObject.Find("ColorWheelHUD");
        _colorWheelHUD.SetActive(false);
    }

    void Update()
    {
        _curposition = transform.position;
        Vector3 distMoved = _curposition - _prevPosition;
        _prevPosition = _curposition;

        cameraWorldAxis.position = cameraWorldAxis.position + new Vector3(0, distMoved.y, 0);
        cameraPanningRevertTarget._gameplayPos =
            cameraPanningRevertTarget._gameplayPos + new Vector3(0, distMoved.y, 0);

        if (LevelManager.freeze_player)
        {
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

    private void RigidGridMove()
    {
        if (_targetLocation != transform.position && _isNotTrackingMovement)
        {
            //print("tracking starts when player starts moving");
            _previousPosForRedo = transform.position;
            _isNotTrackingMovement = false;
            // print("trigger first");
        }

        if (_targetLocation == transform.position && !_isNotTrackingMovement)
        {
            GameState = ScriptableObject.CreateInstance("MoveRedo") as MoveRedo;
            GameState.PlayerInit(this.gameObject, cameraPanningRevertTarget, _targetLocation - _previousPosForRedo,
                _previsouRotationForRedo);
            LevelManager.redoCommandHandler.AddCommand(GameState);
            LevelManager.redoCommandHandler.TransitionToNewGameState();
            _isNotTrackingMovement = true;
            //print("tracking ends when player reaches dest");
            
            // To reset the selected object to the block under the player. If removing the redo code above,
            // leave this line here.
            _paintingSystem.ResetSelectedObject();
        }

        Vector3 newPosition = Vector3.MoveTowards(
            transform.position, _targetLocation, speed * Time.deltaTime
        );

        // The player has reached their movement destination.
        if (Vector3.Distance(newPosition, _targetLocation) <= 0.01f)
        {
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
            //print("recording");
            //transform.rotation = Quaternion.Slerp(
            //    transform.rotation, Quaternion.LookRotation(_moveDirection), 0.5f
            //);
        }

        Vector3 lookDirection = isoCamera.isIntervteredControl ? -_moveDirection : _moveDirection;
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
        RaycastHit ground_hitInfo;
        LayerMask mask = LayerMask.GetMask("Default") | LayerMask.GetMask("IceCube");

        switch (pressedButton)
        {
            case "Up":
                if (!Physics.Raycast(currentTransformPosition + new Vector3(0, 0, 1), Vector3.down, out hitInfo, 1,
                    mask))
                {
                    //Debug.Log("Up bottom is empty");
                    return false;
                }

                ground_hitInfo = hitInfo;
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

                return ValidateFloorMove(ground_hitInfo, Vector3.forward, mask);

            case "Down":
                if (!Physics.Raycast(currentTransformPosition + new Vector3(0, 0, -1), Vector3.down, out hitInfo, 1, mask))
                {
                    //Debug.Log("down bottom is empty");
                    return false;
                }

                ground_hitInfo = hitInfo;
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


                return ValidateFloorMove(ground_hitInfo, Vector3.back, mask);

            case "Left":
                if (!Physics.Raycast(currentTransformPosition + new Vector3(-1, 0, 0), Vector3.down, out hitInfo, 1, mask))
                {
                    // Debug.Log("left bottom is empty ");
                    return false;
                }

                ground_hitInfo = hitInfo;
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


                return ValidateFloorMove(ground_hitInfo, Vector3.left, mask);

            case "Right":
                if (!Physics.Raycast(currentTransformPosition + new Vector3(1, 0, 0), Vector3.down, out hitInfo, 1, mask))
                {
                    //Debug.Log("right bottom is empty");
                    return false;
                }

                ground_hitInfo = hitInfo;
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
                    //Debug.Log("Just right is not empty");
                    return false;
                }

                return ValidateFloorMove(ground_hitInfo, Vector3.right, mask);
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
            return ground.Paint(false); // If false then the floor was not painted.
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
        if (_verticalMovement > 0)
        {
            return isoCamera.isIntervteredControl ? "Down" : "Up";
        }

        if (_verticalMovement < 0)
        {
            return isoCamera.isIntervteredControl ? "Up" : "Down";
        }

        if (_horizontalMovement < 0)
        {
            return isoCamera.isIntervteredControl ? "Right" : "Left";
        }

        if (_horizontalMovement > 0)
        {
            return isoCamera.isIntervteredControl ? "Left" : "Right";
        }

        return "";
    }

    public double GetYLevel()
    {
        return Math.Floor(transform.position.y);
    }
}