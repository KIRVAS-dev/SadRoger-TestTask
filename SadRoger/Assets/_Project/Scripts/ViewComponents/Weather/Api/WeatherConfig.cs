using ContentValidation;
using Core.Gameplay.Weather;
using Infrastructure.ExtendedExceptions;
using UnityEngine;

namespace ViewComponents.Weather
{
    [CreateAssetMenu(menuName = "Configs/Weather Config")]
    public sealed class WeatherConfig
        : ScriptableObject,
          IWeatherSettings,
          IValidatable
    {
        [SerializeField] private WeatherPreset _clear;
        [SerializeField] private WeatherPreset _rain;
        [SerializeField] private WeatherPreset _storm;
        [SerializeField] private WeatherPreset _fog;

        IWeatherPreset IWeatherSettings.GetPreset(WeatherState state)
        {
            return state switch
            {
                WeatherState.Clear => _clear,
                WeatherState.Rain => _rain,
                WeatherState.Storm => _storm,
                WeatherState.Fog => _fog,
                _ => throw new UnhandledWeatherStateException(state)
            };
        }

        public void Validate()
        {
            Guard.AgainstNull(_clear, () => Missing(nameof(_clear)));
            Guard.AgainstNull(_rain, () => Missing(nameof(_rain)));
            Guard.AgainstNull(_storm, () => Missing(nameof(_storm)));
            Guard.AgainstNull(_fog, () => Missing(nameof(_fog)));

            _clear.Validate();
            _rain.Validate();
            _storm.Validate();
            _fog.Validate();

            return;

            ExtendedException Missing(string fieldName) => new MissingWeatherPresetException(fieldName, name);
        }
    }
}
