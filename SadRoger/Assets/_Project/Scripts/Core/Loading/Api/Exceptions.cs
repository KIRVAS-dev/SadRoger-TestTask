using Infrastructure.ExtendedExceptions;

namespace Core.Loading
{
    internal sealed class InvalidLoadingCompletionException : ExtendedException
    {
        internal InvalidLoadingCompletionException()
            : base("loading-1", "Loading is already completed") { }
    }

    internal sealed class InvalidLoadingProgressException : ExtendedException
    {
        internal InvalidLoadingProgressException(float progress)
            : base("loading-2", $"Progress '{progress}' is below the current one or above 1") { }
    }
}
