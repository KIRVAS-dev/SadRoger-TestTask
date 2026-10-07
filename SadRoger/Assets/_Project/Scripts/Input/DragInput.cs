using System;
using Core.Input;
using Core.Loop;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public sealed class DragInput
        : IDragInput,
          IInputTickable
    {
        private const float FallbackScreenDpi = 160f;

        private bool _wasPressed;
        private float _previousPointerX;

        public event Action<float> DragNormalizedDeltaChanged;

        void IInputTickable.Tick()
        {
            Pointer pointer = Pointer.current;
            bool isPointerPressed = pointer != null && pointer.press.isPressed;

            if (!isPointerPressed)
            {
                _wasPressed = false;
                return;
            }

            float pointerX = pointer.position.ReadValue().x;

            if (!_wasPressed)
            {
                _wasPressed = true;
                _previousPointerX = pointerX;
                return;
            }

            float screenDpi = Screen.dpi > 0f
                ? Screen.dpi
                : FallbackScreenDpi;

            float normalizedDelta = (pointerX - _previousPointerX) / screenDpi;

            _previousPointerX = pointerX;

            if (normalizedDelta != 0f)
            {
                DragNormalizedDeltaChanged?.Invoke(normalizedDelta);
            }
        }
    }
}
