using DefaultNamespace;
using UnityEngine;

public class DisplayCheckpointHint : MonoBehaviour
{
    private ControllerUtil _controllerUtil;
    private float _lastTimeButtonPressed;
    private bool _showHint;

    // Start is called before the first frame update
    void Start()
    {
        _controllerUtil = FindObjectOfType<ControllerUtil>();
    }

    // Update is called once per frame
    void Update()
    {
        ListenForControllerInput();
        DisplayHint();
    }

    private void DisplayHint()
    {
        if (_showHint)
        {
            transform.localScale = new Vector3(13.27263f, 8.464599f, 6.2832f);
        }
        else
        {
            transform.localScale = new Vector3(0, 0, 0);
        }
    }

    private void ListenForControllerInput()
    {
        if (_controllerUtil.PressedMovementKeys() ||
            _controllerUtil.GetSwitchRedPressed() ||
            _controllerUtil.GetSwitchYellowPressed() ||
            _controllerUtil.GetSwitchGreenPressed() ||
            _controllerUtil.PressedMovementKeys() ||
            _controllerUtil.PressedPaintKeys()
        )
        {
            _lastTimeButtonPressed = Time.time;
        }

        if ((Time.time - _lastTimeButtonPressed > 5 || Time.time < 5))
        {
            _showHint = true;
        }
        else
        {
            _showHint = false;
        }
    }
}