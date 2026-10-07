using Core.Gameplay.Wind;
using Core.Lifecycle;

namespace Core.Gameplay.Weather
{
    public sealed class WeatherService
        : IWeatherService,
          IPreparationLifecycle
    {
        private readonly IWeatherSettings _settings;
        private readonly IWindStrengthMultiplier _windStrengthMultiplier;
        private readonly WeatherModel _model;

        public WeatherService(
            IWeatherSettings settings,
            IWindStrengthMultiplier windStrengthMultiplier,
            WeatherModel model)
        {
            _settings = settings;
            _windStrengthMultiplier = windStrengthMultiplier;
            _model = model;
        }

        void IPreparationLifecycle.Prepare()
        {
            ApplyState(_model.State.Value);
        }

        void IWeatherService.SetState(WeatherState state)
        {
            ApplyState(state);
        }

        private void ApplyState(WeatherState state)
        {
            IWeatherPreset preset = _settings.GetPreset(state);

            _model.State.Value = state;
            _model.ActivePreset.Value = preset;
            _windStrengthMultiplier.SetMultiplier(preset.WindMultiplier);
        }
    }
}
