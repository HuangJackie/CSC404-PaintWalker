using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class ControllerUtil : MonoBehaviour
    {
        // General objects
        public ChangePerspective isoCamera;

        // ControllerUtil state
        private float _lastTimeButtonPressed;    // Generic button press
        private float _lastTimeMovementPressed;  // Player movement
        private float _lastTimeMenuDpadPressed;  // Menu navigation
        private bool _isMenuOpen;

        // Delay configuration
        [Header("Button Delay Config")]
        public float buttonPressDelayInSeconds = 0.2f;
        public float menuDpadPressDelayInSeconds = 0.2f;

        private bool _togglePanVSPaintSelect; // True when panning

        private void Start()
        {
            _lastTimeButtonPressed = Time.time;
            _lastTimeMovementPressed = _lastTimeButtonPressed;
            _lastTimeMenuDpadPressed = _lastTimeButtonPressed;
            _isMenuOpen = false;
            _togglePanVSPaintSelect = false;
        }

        /*
         * Deprecated Was used for the checkpoint hint.
         */
        public bool PressedMovementKeys()
        {
            return Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        }
        
        /*
         * Deprecated Was used for the checkpoint hint.
         */
        public bool PressedPaintKeys()
        {
            return GetPaintButtonDown() || GetColourWheelPressed();
        }

        public float GetHorizontalAxisRaw()
        {
            if (GetColourWheelPressed() || _isMenuOpen)
            {
                return 0;
            }

            float horizontalAxis = Input.GetAxisRaw("Horizontal");
            return horizontalAxis != 0 && FinishedMovementPressDelay()
                ? horizontalAxis
                : 0;
        }

        public float GetVerticalAxisRaw()
        {
            if (GetColourWheelPressed() || _isMenuOpen)
            {
                return 0;
            }

            float verticalAxis = Input.GetAxisRaw("Vertical");
            return verticalAxis != 0 && FinishedMovementPressDelay()
                ? verticalAxis
                : 0;
        }

        public bool CheckPlayerPressingMovement()
        {
            if (GetColourWheelPressed() || _isMenuOpen)
            {
                return false;
            }
            return (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0);
        }

        public float GetHorizontalPanningAxis()
        {
            if (GetColourWheelPressed() || _isMenuOpen || !_togglePanVSPaintSelect)
            {
                return 0;
            }

            return Input.GetAxis("HorizontalPanning");
        }

        public float GetVerticalPanningAxis()
        {
            if (GetColourWheelPressed() || _isMenuOpen || !_togglePanVSPaintSelect)
            {
                return 0;
            }

            return Input.GetAxis("VerticalPanning");
        }

        /**
         * This is so that a single button press doesn't trigger multiple actions.
         * NOTE: Does not work if Time.timeScale = 0.
         */
        private bool FinishedButtonPressDelay()
        {
            if (Time.time - _lastTimeButtonPressed > buttonPressDelayInSeconds)
            {
                _lastTimeButtonPressed = Time.time;
                return true;
            }
            return false;
        }

        private bool FinishedMovementPressDelay()
        {
            if (Time.time - _lastTimeMovementPressed > buttonPressDelayInSeconds)
            {
                _lastTimeMovementPressed = Time.time;
                return true;
            }
            return false;
        }

        private bool FinishedDpadMenuActionDelay()
        {
            if (Time.time - _lastTimeMenuDpadPressed > menuDpadPressDelayInSeconds)
            {
                _lastTimeMenuDpadPressed = Time.time;
                return true;
            }
            return false;
        }

        public bool GetXAxisPaintSelectAxis(out float axis)
        {
            if (GetColourWheelPressed() || _isMenuOpen || _togglePanVSPaintSelect)
            {
                axis = 0;
                return false;
            }

            axis = Input.GetAxisRaw("XAxisPaintSelect");

            return Input.GetAxisRaw("XAxisPaintSelect") != 0;
        }

        public bool GetZAxisPaintSelectAxis(out float axis)
        {
            if (GetColourWheelPressed() || _isMenuOpen || _togglePanVSPaintSelect)
            {
                axis = 0;
                return false;
            }

            axis = Input.GetAxisRaw("ZAxisPaintSelect");

            return Input.GetAxisRaw("ZAxisPaintSelect") != 0;
        }

        public bool GetColourWheelPressed()
        {
            if (_isMenuOpen)
            {
                return false;
            }

            return Input.GetButton("PaintHUD") || Input.GetKeyDown(KeyCode.Tab);
        }

        public bool GetColourWheelNotPressed()
        {
            return !Input.GetButton("PaintHUD") || Input.GetKeyUp(KeyCode.Tab);
        }

        public float GetColourWheelSelectXAxis()
        {
            return Input.GetAxis("ColourWheelSelectXAxis");
        }

        public float GetColourWheelSelectYAxis()
        {
            return Input.GetAxis("ColourWheelSelectYAxis");
        }

        public bool IsPaintButtonPressed()
        {
            if (GetColourWheelPressed() || _isMenuOpen)
            {
                return false;
            }

            // return Input.GetAxisRaw("Paint") > 0;
            return Input.GetButton("Paint");
        }

        public bool GetPaintButtonDown()
        {
            if (GetColourWheelPressed() || _isMenuOpen)
            {
                return false;
            }

            // return Input.GetAxisRaw("Paint") > 0;
            return Input.GetButtonDown("Paint");
        }

        // public bool GetInteractButtonDown()
        // {
        //     if (GetColourWheelPressed() || _isMenuOpen)
        //     {
        //         return false;
        //     }
        //
        //     return Input.GetButtonDown("Interact");
        // }


        public bool GetRotationChangePressed()
        {
            if (GetColourWheelPressed() || _isMenuOpen)
            {
                return false;
            }

            return Input.GetAxisRaw("RotateCamera") != 0;
        }

        // Return 0 if no rotation changes happening
        public float GetRotationChange()
        {
            if (GetRotationChangePressed())
            {
                return Input.GetAxisRaw("RotateCamera");
            }

            return 0;
        }

        public bool GetMenuButtonPressed()
        {
            return (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Menu")) &&
                   FinishedButtonPressDelay();
        }

        public bool GetConfirmButtonPressed()
        {
            return Input.GetButtonDown("ConfirmMenu") && FinishedButtonPressDelay();
        }

        public bool GetCancelButtonPressed()
        {
            return Input.GetButtonDown("Cancel") && FinishedButtonPressDelay();
        }

        public bool GetGameMenuSelectAxis(out int axis)
        {
            axis = 0;
            if (Input.GetAxis("GameMenuDPadSelectAxis") != 0)
            {
                axis = Input.GetAxis("GameMenuDPadSelectAxis") > 0 ? -1 : 1;
            }
            else if (Input.GetAxis("GameMenuSelectAxis") > 0.3)
            {
                axis = 1;
            }
            else if (Input.GetAxis("GameMenuSelectAxis") < -0.3)
            {
                axis = -1;
            }

            return axis != 0 && FinishedDpadMenuActionDelay();
        }

        public void OpenMenu()
        {
            _isMenuOpen = true;
        }

        public void CloseMenu()
        {
            _isMenuOpen = false;
        }

        public bool GetIsMenuOpen()
        {
            return _isMenuOpen;
        }

        public bool GetSwitchYellowPressed()
        {
            if (_isMenuOpen)
            {
                return false;
            }

            return (Input.GetButtonDown("SwitchYellow") ||
                    Input.GetKeyDown(KeyCode.Alpha1))   &&
                   FinishedButtonPressDelay();
        }

        public bool GetSwitchRedPressed()
        {
            if (_isMenuOpen)
            {
                return false;
            }

            return (Input.GetButtonDown("SwitchRed")  ||
                    Input.GetKeyDown(KeyCode.Alpha2)) &&
                   FinishedButtonPressDelay();
        }

        public bool GetSwitchGreenPressed()
        {
            if (_isMenuOpen)
            {
                return false;
            }

            return (Input.GetButtonDown("SwitchGreen") ||
                    Input.GetKeyDown(KeyCode.Alpha3))  &&
                   FinishedButtonPressDelay();
        }

        public bool GetSwitchBluePressed()
        {
            if (_isMenuOpen)
            {
                return false;
            }

            return (Input.GetButtonDown("SwitchBlue") ||
                    Input.GetKeyDown(KeyCode.Alpha4)) &&
                   FinishedButtonPressDelay();
        }
        
        public bool GetTogglePanVSPaintSelect()
        {
            return (Input.GetButtonDown("TogglePanVSPaintSelect"));
        }

        public void SetTogglePanVSPaintSelect(bool togglePanVSPaintSelect)
        {
            _togglePanVSPaintSelect = togglePanVSPaintSelect;
        }

        public bool isPanningModeOn()
        {
            return _togglePanVSPaintSelect;
        }
    }
}
