using Core.Gameplay.Weather;
using Infrastructure.ExtendedExceptions;

namespace ViewComponents.Weather
{
    internal sealed class MissingWeatherPresetException : ExtendedException
    {
        internal MissingWeatherPresetException(string fieldName, string configName)
            : base("weather-view-1", $"Preset '{fieldName}' is not assigned on '{configName}'") { }
    }

    internal sealed class InvalidWeatherPresetValueException : ExtendedException
    {
        internal InvalidWeatherPresetValueException(
            string presetName,
            string fieldName,
            float value)
            : base("weather-view-2", $"Field '{fieldName}' of preset '{presetName}' has invalid value '{value}'") { }
    }

    internal sealed class UnhandledWeatherStateException : ExtendedException
    {
        internal UnhandledWeatherStateException(WeatherState state)
            : base("weather-view-3", $"WeatherState '{state}' has no preset slot") { }
    }
}
