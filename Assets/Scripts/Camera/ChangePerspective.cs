using System.Collections;
using System.Collections.Generic;
using System;
using DefaultNamespace;
using UnityEngine;
using static GameConstants;

public class ChangePerspective : MonoBehaviour
{
    public LevelManager LevelManager;
    private GameObject _player;
    private ControllerUtil _controllerUtil;

    public float rotationSpeed;
    private float _target_y_angle;
    private float _rot_dest;

    public bool isIntervteredControl;
    public CameraDirection direction;
    public static event Action<CameraDirection> onDirectionChange;
    private bool _changingPersective;

    /**
     * Automatic Panning Variables.
     */
    public float PanningSpeed;

    public GameObject ExitObject;
    private Vector3 _exitPosition;
    private Vector3 _originalPosition;
    private CutSceneManager _cutSceneManager;
    private bool _hasPannedToExit;
    private LevelManager _levelManager;
    private RestartDontDeleteManager _restartDontDeleteManager;

    void Start()
    {
        _player = GameObject.FindWithTag("Player");
        _controllerUtil = FindObjectOfType<ControllerUtil>();

        _target_y_angle = 0f;
        _rot_dest = 0f;
        direction = CameraDirection.N;
        isIntervteredControl = false;
        _changingPersective = false;

        Vector3 exitObjectPosition = ExitObject.transform.position;
        Vector3 position = transform.position;
        _exitPosition = new Vector3(exitObjectPosition.x, position.y, exitObjectPosition.z);
        _originalPosition = position;

        _cutSceneManager = FindObjectOfType<CutSceneManager>();
        _levelManager = FindObjectOfType<LevelManager>();
        _restartDontDeleteManager = FindObjectOfType<RestartDontDeleteManager>();
        print("started again");
    }

    public IEnumerator MoveToPositionAndBack(Vector3 newPosition, Vector3 originalPosition)
    {
        _levelManager.freezePlayer = true;
        while (Vector3.Distance(transform.position, newPosition) > 0.1)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, newPosition, PanningSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = newPosition;

        yield return new WaitForSeconds(2f);
        while (Vector3.Distance(transform.position, originalPosition) > 0.1)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, originalPosition, PanningSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = originalPosition;
        _levelManager.freezePlayer = false;
    }

    void Update()
    {
        if (!_hasPannedToExit
            && (_cutSceneManager == null // For levels with no cutscene
                || (_cutSceneManager != null && !_cutSceneManager.isActiveAndEnabled))) // For levels with a cutscene
        {
            _restartDontDeleteManager = FindObjectOfType<RestartDontDeleteManager>();
            if (!_restartDontDeleteManager.isRestarting)
            {
                StartCoroutine(MoveToPositionAndBack(_exitPosition, _originalPosition));
            }
            _hasPannedToExit = true;
        }

        if (!_levelManager.freezePlayer)
        {
            if (!_changingPersective && (Input.GetKeyDown(KeyCode.Q) ||
                                         _controllerUtil.GetRotationChange() > 0))
            {
                _changingPersective = true;
                _rot_dest = 90;
                _target_y_angle = _target_y_angle < 250f
                    ? _target_y_angle + 90f
                    : 0f;
            }
            else if (!_changingPersective && (Input.GetKeyDown(KeyCode.E) ||
                                              _controllerUtil.GetRotationChange() < 0))
            {
                _changingPersective = true;
                _rot_dest = -90;
                _target_y_angle = _target_y_angle == 0f
                    ? 270f
                    : _target_y_angle - 90f;
            }
        }
    }

    void FixedUpdate()
    {
        if (_changingPersective)
        {
            transform.RotateAround(
                transform.parent.position, Vector3.up, _rot_dest * rotationSpeed * Time.deltaTime
            );

            // Do last-minute calculations to determine if camera reached desired position
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
                }
                else
                {
                    transform.eulerAngles = new Vector3(
                        transform.eulerAngles.x,
                        -Mathf.Abs(_target_y_angle),
                        transform.eulerAngles.z
                    );
                }

                UpdateDirection();
            }
        }
    }


    // Change `direction` clockwise on a compass if rotating clockwise.
    // Otherwise, change direction counter-clockwise on a compass.
    private void UpdateDirection()
    {
        if (_rot_dest > 0) // Rotating clockwise
        {
            direction = direction == CameraDirection.W
                ? CameraDirection.N
                : direction + 1;
        }
        else if (_rot_dest < 0) // Rotating counter-clockwise
        {
            direction = direction == CameraDirection.N
                ? CameraDirection.W
                : direction - 1;
        }

        onDirectionChange?.Invoke(direction);
    }
}