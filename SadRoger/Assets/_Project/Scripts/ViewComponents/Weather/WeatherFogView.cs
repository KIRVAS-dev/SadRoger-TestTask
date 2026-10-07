using Core.Gameplay.Weather;
using UnityEngine;

namespace ViewComponents.Weather
{
    [DisallowMultipleComponent]
    public sealed class WeatherFogView
        : MonoBehaviour,
          IWeatherFogView
    {
        void IWeatherFogView.SetFogDensity(float density)
        {
            RenderSettings.fog = density > 0f;
            RenderSettings.fogDensity = density;
        }
    }
}
