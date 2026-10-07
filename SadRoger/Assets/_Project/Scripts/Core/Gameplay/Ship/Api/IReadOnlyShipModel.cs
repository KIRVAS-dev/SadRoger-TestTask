using Core.Gameplay.Navigation;
using R3;

namespace Core.Gameplay.Ship
{
    public interface IReadOnlyShipModel
    {
        ReadOnlyReactiveProperty<SeaPosition> Position { get; }
        ReadOnlyReactiveProperty<float> Heading { get; }
    }
}
