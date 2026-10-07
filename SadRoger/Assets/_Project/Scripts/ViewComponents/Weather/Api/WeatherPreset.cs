using ContentValidation;
using Core.Gameplay.Weather;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.Weather
{
    [CreateAssetMenu(menuName = "Configs/Weather Preset")]
    public sealed class WeatherPreset
        : ScriptableObject,
          IWeatherPreset,
          IValidatable
    {
        private const float MaxIntensity = 1f;

        [Range(0f, MaxIntensity)]
        [SerializeField] private float _intensity;

        [Tooltip("Exponential squared fog density, 0 disables fog")]
        [SerializeField] private float _fogDensity;

        [Tooltip("Multiplier applied to the base wind strength")]
        [SerializeField] private float _windMultiplier = 1f;

        float IWeatherPreset.Intensity => _intensity;
        float IWeatherPreset.FogDensity => _fogDensity;
        float IWeatherPreset.WindMultiplier => _windMultiplier;

        public void Validate()
        {
            Guard.AgainstNegative(_intensity, () => Invalid(nameof(_intensity), _intensity));
            Guard.AgainstGreaterThan(_intensity, MaxIntensity, () => Invalid(nameof(_intensity), _intensity));
            Guard.AgainstNegative(_fogDensity, () => Invalid(nameof(_fogDensity), _fogDensity));
            Guard.AgainstNegative(_windMultiplier, () => Invalid(nameof(_windMultiplier), _windMultiplier));

            return;

            ExtendedException Invalid(string fieldName, float value) =>
                new InvalidWeatherPresetValueException(name, fieldName, value);
        }
    }
}
