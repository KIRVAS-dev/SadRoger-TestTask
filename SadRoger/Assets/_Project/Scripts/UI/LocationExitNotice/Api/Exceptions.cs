using Infrastructure.ExtendedExceptions;

namespace UI.LocationExitNotice
{
    internal sealed class MissingLocationExitNoticeFieldException : ExtendedException
    {
        internal MissingLocationExitNoticeFieldException(string fieldName, string objectName)
            : base("location-exit-notice-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }
}
