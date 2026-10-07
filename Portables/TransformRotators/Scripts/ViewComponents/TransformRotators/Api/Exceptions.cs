using Infrastructure.ExtendedExceptions;

namespace ViewComponents.TransformRotators
{
    internal sealed class MissingTransformRotatorFieldException : ExtendedException
    {
        internal MissingTransformRotatorFieldException(string fieldName, string objectName)
            : base("transform-rotator-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }
}
