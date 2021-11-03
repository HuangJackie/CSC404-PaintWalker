using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class ChangePerspective : MonoBehaviour
{
    public LevelManager LevelManager;
    private GameObject _player;
    private ControllerUtil _controllerUtil;

    public float rotationSpeed;
    private float _target_y_angle = 0f;
    private float _rot_dest = 0f;
    
    public bool isIntervteredControl;
    private bool _changingPersective;

    void Start()
    {
        isIntervteredControl = false;
        _player = GameObject.FindWithTag("Player");
        _changingPersective = false;
        _controllerUtil = FindObjectOfType<ControllerUtil>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) || _controllerUtil.GetRotationChange() > 0)
        {
            _changingPersective = true;
            _rot_dest = 90;
            if (_target_y_angle < 250f)
            {
                _target_y_angle = _target_y_angle + 90f;
            }
            else
            {
                _target_y_angle = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            _changingPersective = true;
            _rot_dest = -90;

            if (_target_y_angle == 0f)
            {
                _target_y_angle = 270f;
            }
            else
            {
                _target_y_angle = _target_y_angle - 90f;
            }
        }
    }

    void FixedUpdate() { 
        if (_changingPersective)
        {
            transform.RotateAround(transform.parent.position, Vector3.up, _rot_dest * rotationSpeed * Time.deltaTime);
            if (Mathf.Abs(Mathf.Abs(transform.rotation.eulerAngles.y) - _target_y_angle) < 0.1f)
            {
                _changingPersective = false;
                if (transform.rotation.eulerAngles.y > 0)
                {
                    transform.eulerAngles = new Vector3(
                        transform.eulerAngles.x,
                        Mathf.Abs(_target_y_angle),
                        transform.eulerAngles.z
                    );
                } else
                {
                    transform.eulerAngles = new Vector3(
                        transform.eulerAngles.x,
                        -Mathf.Abs(_target_y_angle),
                        transform.eulerAngles.z
                    );
                }

                // Change player movement if camera facing opposite way
                if (_target_y_angle > 160 && _target_y_angle < 200)
                {
                    print("inverting controls");
                    isIntervteredControl = true;
                }
                else
                {
                    isIntervteredControl = false;
                }
            }
        }
    }
}
