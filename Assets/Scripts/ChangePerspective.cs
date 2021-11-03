using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class ChangePerspective : MonoBehaviour
{
    public float rotationSpeed;
    public bool isIntervteredControl;
    public LevelManager LevelManager;
    private float _target_y_angle;
    private float _rot_dest;
    private GameObject _player;
    private bool _changingPersective;
    private ControllerUtil _controllerUtil;
    
    // Start is called before the first frame update
    void Start()
    {
        isIntervteredControl = false;
        _target_y_angle = 180f;
        _rot_dest = 180f;
        _player = GameObject.FindWithTag("Player");
        _changingPersective = false;
        _controllerUtil = FindObjectOfType<ControllerUtil>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E) || _controllerUtil.GetRotationChangePressed())
        {
            Debug.Log("changing persepctive");
            _changingPersective = true;
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
                if (_target_y_angle == 0)
                {
                    _target_y_angle = 180f;
                    isIntervteredControl = false;
                } else
                {
                    _target_y_angle = 0f;
                    isIntervteredControl = true;
                }
            }
        }
    }
}
