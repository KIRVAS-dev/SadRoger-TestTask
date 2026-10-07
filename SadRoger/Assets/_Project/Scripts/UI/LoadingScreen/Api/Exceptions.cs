using Infrastructure.ExtendedExceptions;

namespace UI.LoadingScreen
{
    internal sealed class MissingLoadingScreenFieldException : ExtendedException
    {
        internal MissingLoadingScreenFieldException(string fieldName, string objectName)
            : base("loading-screen-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class InvalidLoadingScreenConfigValueException : ExtendedException
    {
        internal InvalidLoadingScreenConfigValueException(string fieldName, float value)
            : base("loading-screen-2", $"Field '{fieldName}' has invalid value '{value}'") { }
    }

    internal sealed class InvalidLoadingScreenProgressFillException : ExtendedException
    {
        internal InvalidLoadingScreenProgressFillException(string fieldName, string objectName)
            : base("loading-screen-3", $"Image '{fieldName}' on '{objectName}' must be of type Filled") { }
    }
}
