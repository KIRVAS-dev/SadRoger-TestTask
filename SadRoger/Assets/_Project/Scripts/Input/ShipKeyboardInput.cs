using System;
using Core.Input;
using Core.Loop;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public sealed class ShipKeyboardInput
        : IShipControlInput,
          IInputTickable
    {
        private float _throttle;
        private float _steering;

        public event Action ControlChanged;

        float IShipControlInput.Throttle => _throttle;
        float IShipControlInput.Steering => _steering;

        void IInputTickable.Tick()
        {
            Keyboard keyboard = Keyboard.current;

            float throttle = keyboard == null
                ? 0f
                : Axis(keyboard.wKey.isPressed, keyboard.sKey.isPressed);

            float steering = keyboard == null
                ? 0f
                : Axis(keyboard.dKey.isPressed, keyboard.aKey.isPressed);

            if (Mathf.Approximately(throttle, _throttle)
             && Mathf.Approximately(steering, _steering))
            {
                return;
            }

            _throttle = throttle;
            _steering = steering;
            ControlChanged?.Invoke();
        }

        private static float Axis(bool isPositivePressed, bool isNegativePressed)
        {
            float positive = isPositivePressed
                ? 1f
                : 0f;

            float negative = isNegativePressed
                ? 1f
                : 0f;

            return positive - negative;
        }
    }
}
