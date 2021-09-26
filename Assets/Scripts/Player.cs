using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Fluid Movement
    public float velocityIncreaseDamper;
    public float velocityDecreaseDamper;

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
        _horizontalMovement = Input.GetAxis("Horizontal");
        _verticalMovement = Input.GetAxis("Vertical");
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
        switch (pressedButton)
        {
            case "Up":
                if (!Physics.Raycast(currentTransformPosition + new Vector3(0, 0, 1), Vector3.down, out hitInfo, 1))
                {
                    return false;
                }

                return ValidateFloorMove(hitInfo);

            case "Down":
                if (!Physics.Raycast(currentTransformPosition + new Vector3(0, 0, -1), Vector3.down, out hitInfo, 1))
                {
                    return false;
                }

                return ValidateFloorMove(hitInfo);
            case "Left":
                if (!Physics.Raycast(currentTransformPosition + new Vector3(-1, 0, 0), Vector3.down, out hitInfo, 1))
                {
                    return false;
                }

                return ValidateFloorMove(hitInfo);
            case "Right":
                if (!Physics.Raycast(currentTransformPosition + new Vector3(1, 0, 0), Vector3.down, out hitInfo, 1))
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

    // private void FluidMove()
    // {
    //     Vector3 verticalVelocityChange =
    //         transform.forward * _verticalMovement *
    //         (_isVerticalMovementPressed ? velocityIncreaseDamper : velocityDecreaseDamper);
    //     Vector3 HorizontalVelocityChange =
    //         transform.right * _horizontalMovement *
    //         (_isHorizontalMovementPressed ? velocityIncreaseDamper : velocityDecreaseDamper);
    //     _rigidbody.velocity += verticalVelocityChange + HorizontalVelocityChange;
    //
    //     var dir = transform.TransformDirection(Vector3.down);
    //     var velocity = _rigidbody.velocity;
    //     RaycastHit hitInfo;
    //     Ground ground;
    //
    //     // Left
    //     bool isCollided = Physics.Raycast(transform.position - new Vector3(0f, 0.5f, 0.2f), dir, out hitInfo, 1);
    //     if (!isCollided || (hitInfo.collider.gameObject.TryGetComponent(out ground) && !ground._isPainted))
    //     {
    //         if (isCollided && hitInfo.collider.gameObject.TryGetComponent(out ground) && ground.PaintSurface())
    //         {
    //             return;
    //         }
    //
    //         velocity = new Vector3(velocity.x, velocity.y, velocity.z < 0 ? 0 : velocity.z);
    //         _rigidbody.velocity = velocity;
    //     }
    //
    //     // Right
    //     isCollided = Physics.Raycast(transform.position - new Vector3(0f, 0.5f, -0.2f), dir, out hitInfo, 1);
    //     if (!isCollided || (hitInfo.collider.gameObject.TryGetComponent(out ground) && !ground._isPainted))
    //     {
    //         if (isCollided && hitInfo.collider.gameObject.TryGetComponent(out ground) && ground.PaintSurface())
    //         {
    //             return;
    //         }
    //
    //         velocity = new Vector3(velocity.x, velocity.y, velocity.z > 0 ? 0 : velocity.z);
    //         _rigidbody.velocity = velocity;
    //     }
    //
    //
    //     // Up
    //     isCollided = Physics.Raycast(transform.position - new Vector3(0.2f, 0.5f, 0f), dir, out hitInfo, 1);
    //     // Debug.Log(isCollided);
    //
    //     if (!isCollided || (hitInfo.collider.gameObject.TryGetComponent(out ground) && !ground._isPainted))
    //     {
    //         if (isCollided && hitInfo.collider.gameObject.TryGetComponent(out ground) && ground.PaintSurface())
    //         {
    //             return;
    //         }
    //
    //         velocity = new Vector3(velocity.x < 0 ? 0 : velocity.x, velocity.y, velocity.z);
    //         _rigidbody.velocity = velocity;
    //     }
    //
    //
    //     // Down
    //     isCollided = Physics.Raycast(transform.position - new Vector3(-0.2f, 0.5f, 0f), dir, out hitInfo, 1);
    //     if (!isCollided || (hitInfo.collider.gameObject.TryGetComponent(out ground) && !ground._isPainted))
    //     {
    //         if (isCollided && hitInfo.collider.gameObject.TryGetComponent(out ground) && ground.PaintSurface())
    //         {
    //             Debug.Log("early return");
    //             return;
    //         }
    //
    //         velocity = new Vector3(velocity.x > 0 ? 0 : velocity.x, velocity.y, velocity.z);
    //         _rigidbody.velocity = velocity;
    //     }
    // }
}