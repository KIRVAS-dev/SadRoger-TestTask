namespace Core.Gameplay.Weather
{
    public interface IWeatherPreset
    {
        float Intensity { get; }
        float FogDensity { get; }
        float WindMultiplier { get; }
    }
}
