using System;
using ContentValidation;
using Core.Gameplay.Weather;
using Infrastructure.ExtendedExceptions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WeatherHud
{
    public sealed class WeatherHudView
        : MonoBehaviour,
          IWeatherHudView,
          IValidatable
    {
        private const string PercentFormat = "{0}%";
        private const float PercentFactor = 100f;

        [SerializeField] private TextMeshProUGUI _intensityText;
        [SerializeField] private Button _clearButton;
        [SerializeField] private Button _rainButton;
        [SerializeField] private Button _stormButton;
        [SerializeField] private Button _fogButton;
        [SerializeField] private Color _activeColor = Color.white;
        [SerializeField] private Color _inactiveColor = Color.gray;

        public event Action<WeatherState> StateClicked;

        private void Awake()
        {
            _clearButton.onClick.AddListener(OnClearClicked);
            _rainButton.onClick.AddListener(OnRainClicked);
            _stormButton.onClick.AddListener(OnStormClicked);
            _fogButton.onClick.AddListener(OnFogClicked);
        }

        private void OnDestroy()
        {
            _clearButton.onClick.RemoveListener(OnClearClicked);
            _rainButton.onClick.RemoveListener(OnRainClicked);
            _stormButton.onClick.RemoveListener(OnStormClicked);
            _fogButton.onClick.RemoveListener(OnFogClicked);
        }

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_intensityText, () => Missing(nameof(_intensityText)));
            Guard.AgainstNull(_clearButton, () => Missing(nameof(_clearButton)));
            Guard.AgainstNull(_rainButton, () => Missing(nameof(_rainButton)));
            Guard.AgainstNull(_stormButton, () => Missing(nameof(_stormButton)));
            Guard.AgainstNull(_fogButton, () => Missing(nameof(_fogButton)));

            return;

            ExtendedException Missing(string fieldName) => new MissingWeatherHudFieldException(fieldName, gameObject.name);
        }

        void IWeatherHudView.SetActiveState(WeatherState state)
        {
            Paint(_clearButton, state == WeatherState.Clear);
            Paint(_rainButton, state == WeatherState.Rain);
            Paint(_stormButton, state == WeatherState.Storm);
            Paint(_fogButton, state == WeatherState.Fog);
        }

        void IWeatherHudView.SetIntensity(float intensity)
        {
            _intensityText.SetText(PercentFormat, Mathf.Round(intensity * PercentFactor));
        }

        private void Paint(Button button, bool isActive)
        {
            button.image.color = isActive
                ? _activeColor
                : _inactiveColor;
        }

        private void OnClearClicked()
        {
            StateClicked?.Invoke(WeatherState.Clear);
        }

        private void OnRainClicked()
        {
            StateClicked?.Invoke(WeatherState.Rain);
        }

        private void OnStormClicked()
        {
            StateClicked?.Invoke(WeatherState.Storm);
        }

        private void OnFogClicked()
        {
            StateClicked?.Invoke(WeatherState.Fog);
        }
    }
}
