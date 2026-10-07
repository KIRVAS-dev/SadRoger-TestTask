using Core.Gameplay.Navigation;
using R3;

namespace Core.Gameplay.Ship
{
    public sealed class ShipModel : IReadOnlyShipModel
    {
        public ReactiveProperty<SeaPosition> Position { get; } = new ReactiveProperty<SeaPosition>();
        public ReactiveProperty<float> Heading { get; } = new ReactiveProperty<float>();

        ReadOnlyReactiveProperty<SeaPosition> IReadOnlyShipModel.Position => Position;
        ReadOnlyReactiveProperty<float> IReadOnlyShipModel.Heading => Heading;
    }
}
