using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class ControllerUtil : MonoBehaviour
    {
        public float buttonPressDelayInSeconds;
        private float _lastTimeButtonPressed;

        private void Start()
        {
            _lastTimeButtonPressed = Time.time;
        }

        public float GetHorizontalAxisRaw()
        {
            float horizontalAxis = Input.GetAxisRaw("Horizontal");
            return FinishedMovementDelay(horizontalAxis) ? horizontalAxis : 0;
        }

        public float GetVerticalAxisRaw()
        {
            float verticalAxis = Input.GetAxisRaw("Vertical");
            return FinishedMovementDelay(verticalAxis) ? verticalAxis : 0;
        }

        public bool PaintSelectionUIToggled()
        {
            return Input.GetButtonDown("PaintSelectionUI");
        }

        /**
         * This is so that a single movement press doesn't trigger two blocks of movement.
         */
        private bool FinishedMovementDelay(float movement)
        {
            if (movement != 0 && Time.time - _lastTimeButtonPressed > buttonPressDelayInSeconds)
            {
                _lastTimeButtonPressed = Time.time;
                return true;
            }

            return false;
        }

        public bool GetXAxisPaintSelectAxis(out float axis)
        {
            axis = Input.GetAxisRaw("XAxisPaintSelect");
            return FinishedMovementDelay(axis);
        }

        public bool GetZAxisPaintSelectAxis(out float axis)
        {
            axis = Input.GetAxisRaw("ZAxisPaintSelect");
            return FinishedMovementDelay(axis);
        }
        
        public bool GetYAxisPaintSelectAxis(out float axis)
        {
            axis = Input.GetAxisRaw("YAxisPaintSelect");
            return FinishedMovementDelay(axis);
        }

        public bool GetPaintButtonDown()
        {
            return Input.GetButtonDown("Paint");
        }

        public bool GetInteractButtonDown()
        {
            return Input.GetButtonDown("Interact");
        }
    }
}