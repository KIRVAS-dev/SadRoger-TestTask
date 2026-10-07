using Infrastructure.ExtendedExceptions;

namespace UI.WeatherHud
{
    internal sealed class MissingWeatherHudFieldException : ExtendedException
    {
        internal MissingWeatherHudFieldException(string fieldName, string objectName)
            : base("weather-hud-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }
}
