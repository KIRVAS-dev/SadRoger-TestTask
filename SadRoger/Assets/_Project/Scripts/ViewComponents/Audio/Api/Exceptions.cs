using Infrastructure.ExtendedExceptions;

namespace ViewComponents.Audio
{
    internal sealed class MissingStudioListenerAnchorException : ExtendedException
    {
        internal MissingStudioListenerAnchorException(string objectName)
            : base("studio-listener-view-1", $"IStudioListenerAnchor was not injected on '{objectName}'") { }
    }
}
