using R3;

namespace Core.Gameplay.LocationBoundary
{
    public sealed class LocationBoundaryModel : IReadOnlyLocationBoundaryModel
    {
        public ReactiveProperty<LocationBoundaryState> State { get; } =
            new ReactiveProperty<LocationBoundaryState>(LocationBoundaryState.Inside);

        public ReactiveProperty<float> CountdownRemaining { get; } = new ReactiveProperty<float>();

        ReadOnlyReactiveProperty<LocationBoundaryState> IReadOnlyLocationBoundaryModel.State => State;
        ReadOnlyReactiveProperty<float> IReadOnlyLocationBoundaryModel.CountdownRemaining => CountdownRemaining;
    }
}
