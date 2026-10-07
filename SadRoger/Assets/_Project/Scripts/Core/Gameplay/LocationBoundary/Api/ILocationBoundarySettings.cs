namespace Core.Gameplay.LocationBoundary
{
    public interface ILocationBoundarySettings
    {
        float Radius { get; }
        float WarningDelay { get; }
        float ExitCountdown { get; }
    }
}
