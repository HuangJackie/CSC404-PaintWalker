using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using static GameConstants;

public class CameraRotation : MonoBehaviour
{
    public float speed;
    public float controllerPanningSpeed;
    public float transitionBackSpeed;
    public LevelManager LevelManager;
    public ChangePerspective isoCamera;
    public Vector3 _gameplayPos;

    public bool _wasPanning;
    private bool _transitioning_back;
    private Vector3 _panningPos;
    private Vector3 _initialClickPosition;
    
    Vector3 forward, right;

    private ControllerUtil _controllerUtil;

    public bool IsPanning()
    {
        return LevelManager.IsPanning();
    }

    void Start()
    {
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        _wasPanning = false;
        _transitioning_back = false;

        _controllerUtil = FindObjectOfType<ControllerUtil>();
    }

    void LateUpdate()
    {
        if (_wasPanning && !LevelManager.IsPanning())
        {
            // transitioning back
            transform.parent.parent.position = Vector3.Lerp(
                transform.parent.parent.position, _gameplayPos, transitionBackSpeed * Time.deltaTime
            );
            
            if (Vector3.Distance(transform.parent.parent.position, _gameplayPos) >= 0.01f)
            {
                _transitioning_back = true;
            }
            else
            {
                transform.parent.parent.position = _gameplayPos;
                _transitioning_back = false;
                _wasPanning = false;
            }
        }

        if (!_transitioning_back)
        {
            if (!LevelManager.IsPanning())
            {
                _gameplayPos = transform.parent.parent.position;
            }

            if (Input.GetMouseButtonDown(2))
            {
                _initialClickPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(2))
            {
                LevelManager.SetIsPanning(true);
                _wasPanning = true;
                _panningPos = transform.parent.parent.position;
                Vector3 distanceMoved = Input.mousePosition - _initialClickPosition;

                Vector3 rightMovement = Vector3.zero;
                Vector3 upMovement = Vector3.zero;
                switch (isoCamera.direction)
                {
                    case CameraDirection.N:
                        rightMovement = right * speed * Time.deltaTime * -distanceMoved.x;
                        upMovement = forward * speed * Time.deltaTime * -distanceMoved.y;
                        break;
                    case CameraDirection.E:
                        rightMovement = right * speed * Time.deltaTime * -distanceMoved.y;
                        upMovement = forward * speed * Time.deltaTime * distanceMoved.x;
                        break;
                    case CameraDirection.S:
                        rightMovement = right * speed * Time.deltaTime * distanceMoved.x;
                        upMovement = forward * speed * Time.deltaTime * distanceMoved.y;
                        break;
                    case CameraDirection.W:
                        rightMovement = right * speed * Time.deltaTime * distanceMoved.y;
                        upMovement = forward * speed * Time.deltaTime * -distanceMoved.x;
                        break;
                    default:  // Same as CameraDirection.N
                        rightMovement = right * speed * Time.deltaTime * -distanceMoved.x;
                        upMovement = forward * speed * Time.deltaTime * -distanceMoved.y;
                        break;
                }

                transform.transform.parent.parent.position += rightMovement;
                transform.transform.parent.parent.position += upMovement;
                _initialClickPosition = Input.mousePosition;
                return;
            }

            float horizontalPanning = _controllerUtil.GetHorizontalPanningAxis() * controllerPanningSpeed;
            float verticalPanning = _controllerUtil.GetVerticalPanningAxis() * controllerPanningSpeed;
            print(horizontalPanning);
            print(verticalPanning);

            if (isoCamera.isIntervteredControl)
            {
                horizontalPanning = -horizontalPanning;
                verticalPanning = -verticalPanning;
            }
            
            if (horizontalPanning != 0 || verticalPanning != 0)
            {
                _panningPos = transform.parent.parent.position;
                _wasPanning = true;
                LevelManager.SetIsPanning(true);
            }

            transform.transform.parent.parent.position += new Vector3(verticalPanning, 0, horizontalPanning);
        }
    }
}
