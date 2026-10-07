namespace Core.Gameplay.Weather
{
    public interface IWeatherSettings
    {
        IWeatherPreset GetPreset(WeatherState state);
    }
}
