using Infrastructure.ExtendedExceptions;

namespace Core.Bootstrap
{
    internal sealed class MissingCoreScopeFieldException : ExtendedException
    {
        internal MissingCoreScopeFieldException(string fieldName, string objectName)
            : base("core-scope-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }
}
