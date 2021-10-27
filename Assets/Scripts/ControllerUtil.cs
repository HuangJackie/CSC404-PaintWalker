using System;
using UnityEditor.Rendering;
using UnityEngine;

namespace DefaultNamespace
{
    public class ControllerUtil : MonoBehaviour
    {
        public float delayBetweenMovementsInSeconds;
        private float _lastTimePressed;

        private void Start()
        {
            _lastTimePressed = Time.time;
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
            if (movement != 0 && Time.time - _lastTimePressed > delayBetweenMovementsInSeconds)
            {
                _lastTimePressed = Time.time;
                return true;
            }

            return false;
        }
    }
}