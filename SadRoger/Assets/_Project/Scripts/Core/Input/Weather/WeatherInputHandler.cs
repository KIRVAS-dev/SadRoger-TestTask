using Core.Gameplay.Weather;
using Core.Lifecycle;

namespace Core.Input.Weather
{
    public sealed class WeatherInputHandler : ISubscriptionLifecycle
    {
        private readonly IWeatherInput _input;
        private readonly IWeatherService _weatherService;

        public WeatherInputHandler(IWeatherInput input, IWeatherService weatherService)
        {
            _input = input;
            _weatherService = weatherService;
        }

        void ISubscriptionLifecycle.Start()
        {
            ApplyState();

            _input.StateChanged += ApplyState;
        }

        void ISubscriptionLifecycle.Stop()
        {
            _input.StateChanged -= ApplyState;
        }

        private void ApplyState()
        {
            _weatherService.SetState(_input.State);
        }
    }
}
