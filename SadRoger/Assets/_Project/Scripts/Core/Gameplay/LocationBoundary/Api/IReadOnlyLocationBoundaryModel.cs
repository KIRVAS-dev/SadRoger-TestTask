using R3;

namespace Core.Gameplay.LocationBoundary
{
    public interface IReadOnlyLocationBoundaryModel
    {
        ReadOnlyReactiveProperty<LocationBoundaryState> State { get; }
        ReadOnlyReactiveProperty<float> CountdownRemaining { get; }
    }
}
