using Infrastructure.ExtendedExceptions;

namespace ViewComponents.LocationBoundary
{
    internal sealed class InvalidLocationBoundaryValueException : ExtendedException
    {
        internal InvalidLocationBoundaryValueException(string fieldName, float value)
            : base("location-boundary-view-1", $"Field '{fieldName}' has invalid value '{value}'") { }
    }

    internal sealed class MissingLocationBoundaryFieldException : ExtendedException
    {
        internal MissingLocationBoundaryFieldException(string fieldName, string objectName)
            : base("location-boundary-view-2", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class InvalidLocationBoundaryRingValueException : ExtendedException
    {
        internal InvalidLocationBoundaryRingValueException(
            string fieldName,
            string objectName,
            int value)
            : base("location-boundary-view-3", $"Field '{fieldName}' on '{objectName}' has invalid value '{value}'") { }
    }
}
