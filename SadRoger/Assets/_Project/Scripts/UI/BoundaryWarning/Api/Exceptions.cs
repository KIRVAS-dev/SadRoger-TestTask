using Infrastructure.ExtendedExceptions;

namespace UI.BoundaryWarning
{
    internal sealed class MissingBoundaryWarningFieldException : ExtendedException
    {
        internal MissingBoundaryWarningFieldException(string fieldName, string objectName)
            : base("boundary-warning-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }
}
