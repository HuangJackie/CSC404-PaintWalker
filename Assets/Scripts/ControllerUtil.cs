using System;
using UnityEditor.Rendering;
using UnityEngine;

namespace DefaultNamespace
{
    public class ControllerUtil : MonoBehaviour
    {
        public float DelayBetweenMovementsInSeconds;
        private float _horizontalAxis;
        private float _verticalAxis;
        private float _lastTimePressed;

        private void Start()
        {
            _lastTimePressed = Time.time;
        }

        private void FixedUpdate()
        {
            _horizontalAxis = Input.GetAxisRaw("Horizontal");
            _verticalAxis = Input.GetAxisRaw("Vertical");
        }

        public float GetHorizontalAxisRaw()
        {
            return FinishedMovementDelay(_horizontalAxis) ? _horizontalAxis : 0;
        }

        public float GetVerticalAxisRaw()
        {
            return FinishedMovementDelay(_verticalAxis) ? _verticalAxis : 0;
        }

        /**
         * This is so that a single movement press doesn't trigger two blocks of movement.
         */
        private bool FinishedMovementDelay(float movement)
        {
            // print(Time.time );
            if (movement != 0 && Time.time - _lastTimePressed > DelayBetweenMovementsInSeconds)
            {
                print("yup");

                _lastTimePressed = Time.time;
                return true;
            }

            // print("not yet: " + Time.fixedDeltaTime + " " + _lastTimePressed);
            return false;
        }
    }
}