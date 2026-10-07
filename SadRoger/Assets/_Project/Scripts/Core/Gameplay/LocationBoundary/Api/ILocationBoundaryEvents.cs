using System;

namespace Core.Gameplay.LocationBoundary
{
    public interface ILocationBoundaryEvents
    {
        event Action LocationExited;
    }
}
