using Infrastructure.ExtendedExceptions;

namespace Core.Gameplay.LocationBoundary
{
    internal sealed class UnhandledLocationBoundaryStateException : ExtendedException
    {
        internal UnhandledLocationBoundaryStateException(LocationBoundaryState state)
            : base("location-boundary-1", $"LocationBoundaryState '{state}' is not handled") { }
    }
}
