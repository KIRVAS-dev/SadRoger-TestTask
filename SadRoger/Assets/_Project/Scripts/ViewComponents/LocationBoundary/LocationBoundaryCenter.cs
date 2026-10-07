using Core.Gameplay.LocationBoundary;
using Core.Gameplay.Navigation;
using UnityEngine;

namespace ViewComponents.LocationBoundary
{
    [DisallowMultipleComponent]
    public sealed class LocationBoundaryCenter
        : MonoBehaviour,
          ILocationBoundaryCenterProvider
    {
        SeaPosition ILocationBoundaryCenterProvider.Center => new SeaPosition(transform.position.x, transform.position.z);
    }
}
