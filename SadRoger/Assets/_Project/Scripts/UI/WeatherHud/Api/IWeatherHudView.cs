using System;
using Core.Gameplay.Weather;

namespace UI.WeatherHud
{
    public interface IWeatherHudView
    {
        event Action<WeatherState> StateClicked;

        void SetActiveState(WeatherState state);
        void SetIntensity(float intensity);
    }
}
