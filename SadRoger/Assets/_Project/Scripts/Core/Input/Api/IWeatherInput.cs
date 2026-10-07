using System;
using Core.Gameplay.Weather;

namespace Core.Input
{
    public interface IWeatherInput
    {
        event Action StateChanged;

        WeatherState State { get; }
    }
}
