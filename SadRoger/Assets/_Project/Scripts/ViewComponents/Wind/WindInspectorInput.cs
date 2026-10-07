using System;
using Core.Input;
using Core.Loop;
using UnityEngine;

namespace ViewComponents.Wind
{
    [DisallowMultipleComponent]
    public sealed class WindInspectorInput
        : MonoBehaviour,
          IWindInput,
          IInputTickable
    {
        [Tooltip("Direction the wind blows from, degrees clockwise from world +Z")]
        [Range(0f, 360f)]
        [SerializeField] private float _direction;
        [Range(0f, 1f)]
        [SerializeField] private float _strength = 0.5f;

        private float _appliedDirection;
        private float _appliedStrength;

        public event Action DirectionChanged;
        public event Action StrengthChanged;

        float IWindInput.Direction => _direction;
        float IWindInput.Strength => _strength;

        private void Awake()
        {
            _appliedDirection = _direction;
            _appliedStrength = _strength;
        }

        void IInputTickable.Tick()
        {
            if (!Mathf.Approximately(_direction, _appliedDirection))
            {
                _appliedDirection = _direction;
                DirectionChanged?.Invoke();
            }

            if (!Mathf.Approximately(_strength, _appliedStrength))
            {
                _appliedStrength = _strength;
                StrengthChanged?.Invoke();
            }
        }
    }
}
