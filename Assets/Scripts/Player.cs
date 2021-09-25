using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float velocityIncreaseDamper;
    public float velocityDecreaseDamper;

    private float _horizontalMovement;
    private float _verticalMovement;
    private bool _isHorizontalMovementPressed;
    private bool _isVerticalMovementPressed;

    private Transform _transform;
    private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        _horizontalMovement = Input.GetAxis("Vertical");
        _verticalMovement = Input.GetAxis("Horizontal");
        _isHorizontalMovementPressed = Input.GetButton("Vertical");
        _isVerticalMovementPressed = Input.GetButton("Horizontal");
    }

    private void FixedUpdate()
    {
        Vector3 verticalVelocityChange =
            transform.forward * _verticalMovement *
            (_isVerticalMovementPressed ? velocityIncreaseDamper : velocityDecreaseDamper);
        Vector3 HorizontalVelocityChange =
            transform.right * -_horizontalMovement *
            (_isHorizontalMovementPressed ? velocityIncreaseDamper : velocityDecreaseDamper);
        _rigidbody.velocity += verticalVelocityChange + HorizontalVelocityChange;

        var dir = transform.TransformDirection(Vector3.down);
        var velocity = _rigidbody.velocity;
        RaycastHit hitInfo;
        Ground ground;

        // Left
        bool isCollided = Physics.Raycast(transform.position - new Vector3(0f, 0.5f, 0.2f), dir, out hitInfo, 1);
        if (!isCollided || (hitInfo.collider.gameObject.TryGetComponent(out ground) && !ground._isPainted))
        {
            if (isCollided && hitInfo.collider.gameObject.TryGetComponent(out ground) && ground.PaintSurface())
            {
                return;
            }
            velocity = new Vector3(velocity.x, velocity.y, velocity.z < 0 ? 0 : velocity.z);
            _rigidbody.velocity = velocity;
        }

        // Right
        isCollided = Physics.Raycast(transform.position - new Vector3(0f, 0.5f, -0.2f), dir, out hitInfo, 1);
        if (!isCollided || (hitInfo.collider.gameObject.TryGetComponent(out ground) && !ground._isPainted))
        {
            if (isCollided && hitInfo.collider.gameObject.TryGetComponent(out ground) && ground.PaintSurface())
            {
                return;
            }
            velocity = new Vector3(velocity.x, velocity.y, velocity.z > 0 ? 0 : velocity.z);
            _rigidbody.velocity = velocity;
        }


        // Up
        isCollided = Physics.Raycast(transform.position - new Vector3(0.2f, 0.5f, 0f), dir, out hitInfo, 1);
        Debug.Log(isCollided);

        if (!isCollided || (hitInfo.collider.gameObject.TryGetComponent(out ground) && !ground._isPainted))
        {
            if (isCollided && hitInfo.collider.gameObject.TryGetComponent(out ground) && ground.PaintSurface())
            {
                return;
            }
            velocity = new Vector3(velocity.x < 0 ? 0 : velocity.x, velocity.y, velocity.z);
            _rigidbody.velocity = velocity;
        }


        // Down
        isCollided = Physics.Raycast(transform.position - new Vector3(-0.2f, 0.5f, 0f), dir, out hitInfo, 1);
        if (!isCollided || (hitInfo.collider.gameObject.TryGetComponent(out ground) && !ground._isPainted))
        {
            if (isCollided && hitInfo.collider.gameObject.TryGetComponent(out ground) && ground.PaintSurface())
            {
                return;
            }
            velocity = new Vector3(velocity.x > 0 ? 0 : velocity.x, velocity.y, velocity.z);
            _rigidbody.velocity = velocity;
        }
    }
}