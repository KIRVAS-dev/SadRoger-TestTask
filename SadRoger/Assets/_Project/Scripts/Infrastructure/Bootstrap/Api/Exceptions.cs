using Infrastructure.ExtendedExceptions;

namespace Infrastructure.Bootstrap
{
    internal sealed class MissingLoadingScreenViewException : ExtendedException
    {
        internal MissingLoadingScreenViewException(string fieldName, string objectName)
            : base("infrastructure-bootstrap-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }
}
