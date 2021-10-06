using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Fluid Movement
    public float velocityIncreaseDamper;
    public float velocityDecreaseDamper;

    public LevelManager LevelManager;

    private float _horizontalMovement;
    private float _verticalMovement;
    private bool _isHorizontalMovementPressed;
    private bool _isVerticalMovementPressed;
    private Rigidbody _rigidbody;

    // Rigid Grid Movement
    public float speed;
    private Vector3 _targetLocation;

    private UpdateUI _updateUI;
    private Transform _transform;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody>();
        // _updateUI = FindObjectOfType()<UpdateUI>();

        _targetLocation = transform.position;
        // _isMoving = false;
    }

    // Update is called once per frame
    void Update()
    {
        _horizontalMovement = Input.GetAxisRaw("Horizontal");
        _verticalMovement = Input.GetAxisRaw("Vertical");
        _isHorizontalMovementPressed = Input.GetButton("Horizontal");
        _isVerticalMovementPressed = Input.GetButton("Vertical");

        RigidGridMove();
    }

    private void FixedUpdate()
    {
        // FluidMove();
    }

    private void RigidGridMove()
    {
        Vector3 newPosition = Vector3.MoveTowards(transform.position, _targetLocation, speed * Time.deltaTime);
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
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movDirection), 0.5f);
        }
    }

    private void SetNewTargetLocation(Vector3 currentTransformPosition)
    {
        string pressedButton = GetCurrentlyPressedDirection();

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
        LayerMask mask = LayerMask.GetMask("Default");
        switch (pressedButton)
        {
            case "Up":
                if (!Physics.Raycast(currentTransformPosition + new Vector3(0, 0, 1), Vector3.down, out hitInfo, 1,
                    mask))
                {
                    return false;
                }

                return ValidateFloorMove(hitInfo);

            case "Down":
                if (!Physics.Raycast(currentTransformPosition + new Vector3(0, 0, -1), Vector3.down, out hitInfo, 1,
                    mask))
                {
                    return false;
                }

                return ValidateFloorMove(hitInfo);
            case "Left":

                if (!Physics.Raycast(currentTransformPosition + new Vector3(-1, 0, 0), Vector3.down, out hitInfo, 1,
                    mask))
                {
                    return false;
                }

                return ValidateFloorMove(hitInfo);
            case "Right":
                if (!Physics.Raycast(currentTransformPosition + new Vector3(1, 0, 0), Vector3.down, out hitInfo, 1,
                    mask))
                {
                    return false;
                }

                return ValidateFloorMove(hitInfo);
        }

        return false;
    }

    private bool ValidateFloorMove(RaycastHit hitInfo)
    {
        if (hitInfo.transform.position.y > 1)
        {
            // Going to hit a wall, don't move.
            return false;
        }

        Ground ground;
        if (hitInfo.collider.gameObject.TryGetComponent(out ground))
        {
            if (ground._isPainted)
            {
                // Painted surface, can move.
                return true;
            }

            // Try to paint.
            return ground.PaintSurface(); // If false then the floor was not painted.
        }

        return false;
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