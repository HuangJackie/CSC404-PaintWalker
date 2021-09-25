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
    }
}
