using System;
using Core.Gameplay.Weather;
using Core.Lifecycle;
using R3;

namespace UI.WeatherHud
{
    public sealed class WeatherHudPresenter : ISubscriptionLifecycle
    {
        private readonly IWeatherHudView _view;
        private readonly IReadOnlyWeatherModel _model;
        private readonly IWeatherService _weatherService;

        private IDisposable _subscriptions;

        public WeatherHudPresenter(
            IWeatherHudView view,
            IReadOnlyWeatherModel model,
            IWeatherService weatherService)
        {
            _view = view;
            _model = model;
            _weatherService = weatherService;
        }

        void ISubscriptionLifecycle.Start()
        {
            _subscriptions = Disposable.Combine(
                _model.State.Subscribe(_view.SetActiveState),
                _model.ActivePreset.Subscribe(OnPresetChanged)
            );

            _view.StateClicked += OnStateClicked;
        }

        void ISubscriptionLifecycle.Stop()
        {
            _view.StateClicked -= OnStateClicked;
            _subscriptions?.Dispose();
        }

        private void OnPresetChanged(IWeatherPreset preset)
        {
            _view.SetIntensity(preset.Intensity);
        }

        private void OnStateClicked(WeatherState state)
        {
            _weatherService.SetState(state);
        }
    }
}
