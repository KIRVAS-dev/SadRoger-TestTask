using Infrastructure.ExtendedExceptions;

namespace Infrastructure.Bootstrap
{
    internal sealed class MissingStudioListenerAnchorTransformException : ExtendedException
    {
        internal MissingStudioListenerAnchorTransformException()
            : base("infrastructure-bootstrap-1", "StudioListenerAnchor transform is not assigned") { }
    }

    internal sealed class MissingLoadingScreenViewException : ExtendedException
    {
        internal MissingLoadingScreenViewException(string fieldName, string objectName)
            : base("infrastructure-bootstrap-2", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }
}
