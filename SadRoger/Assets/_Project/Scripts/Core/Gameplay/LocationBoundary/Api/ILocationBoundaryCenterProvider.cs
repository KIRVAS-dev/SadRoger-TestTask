using Core.Gameplay.Navigation;

namespace Core.Gameplay.LocationBoundary
{
    public interface ILocationBoundaryCenterProvider
    {
        SeaPosition Center { get; }
    }
}
