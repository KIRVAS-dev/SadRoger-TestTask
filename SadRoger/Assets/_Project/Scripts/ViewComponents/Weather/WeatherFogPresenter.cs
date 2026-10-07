using System;
using Core.Gameplay.Weather;
using Core.Lifecycle;
using R3;

namespace ViewComponents.Weather
{
    public sealed class WeatherFogPresenter : ISubscriptionLifecycle
    {
        private readonly IWeatherFogView _view;
        private readonly IReadOnlyWeatherModel _model;

        private IDisposable _subscription;

        public WeatherFogPresenter(IWeatherFogView view, IReadOnlyWeatherModel model)
        {
            _view = view;
            _model = model;
        }

        void ISubscriptionLifecycle.Start()
        {
            _subscription = _model.ActivePreset.Subscribe(OnPresetChanged);
        }

        void ISubscriptionLifecycle.Stop()
        {
            _subscription?.Dispose();
        }

        private void OnPresetChanged(IWeatherPreset preset)
        {
            _view.SetFogDensity(preset.FogDensity);
        }
    }
}
