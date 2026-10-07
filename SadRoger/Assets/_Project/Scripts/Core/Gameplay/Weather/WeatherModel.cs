using R3;

namespace Core.Gameplay.Weather
{
    public sealed class WeatherModel : IReadOnlyWeatherModel
    {
        public ReactiveProperty<WeatherState> State { get; } = new ReactiveProperty<WeatherState>(WeatherState.Clear);
        public ReactiveProperty<IWeatherPreset> ActivePreset { get; } = new ReactiveProperty<IWeatherPreset>();

        ReadOnlyReactiveProperty<WeatherState> IReadOnlyWeatherModel.State => State;
        ReadOnlyReactiveProperty<IWeatherPreset> IReadOnlyWeatherModel.ActivePreset => ActivePreset;
    }
}
