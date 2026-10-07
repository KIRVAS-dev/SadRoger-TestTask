using R3;

namespace Core.Gameplay.Weather
{
    public interface IReadOnlyWeatherModel
    {
        ReadOnlyReactiveProperty<WeatherState> State { get; }
        ReadOnlyReactiveProperty<IWeatherPreset> ActivePreset { get; }
    }
}
