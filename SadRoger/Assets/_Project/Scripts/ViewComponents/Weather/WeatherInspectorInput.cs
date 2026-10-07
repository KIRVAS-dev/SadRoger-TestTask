using System;
using Core.Gameplay.Weather;
using Core.Input;
using Core.Loop;
using UnityEngine;

namespace ViewComponents.Weather
{
    [DisallowMultipleComponent]
    public sealed class WeatherInspectorInput
        : MonoBehaviour,
          IWeatherInput,
          IInputTickable
    {
        [SerializeField] private WeatherState _state = WeatherState.Clear;

        private WeatherState _appliedState;

        public event Action StateChanged;

        WeatherState IWeatherInput.State => _state;

        private void Awake()
        {
            _appliedState = _state;
        }

        void IInputTickable.Tick()
        {
            if (_state == _appliedState)
            {
                return;
            }

            _appliedState = _state;
            StateChanged?.Invoke();
        }
    }
}
