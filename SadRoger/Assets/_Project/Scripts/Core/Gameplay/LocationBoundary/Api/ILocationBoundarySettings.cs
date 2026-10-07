namespace Core.Gameplay.LocationBoundary
{
    public interface ILocationBoundarySettings
    {
        float CenterX { get; }
        float CenterZ { get; }
        float Radius { get; }
        float WarningDelay { get; }
        float ExitCountdown { get; }
    }
}
