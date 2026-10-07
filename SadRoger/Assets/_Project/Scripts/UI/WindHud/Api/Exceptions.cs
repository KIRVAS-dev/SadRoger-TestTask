using Infrastructure.ExtendedExceptions;

namespace UI.WindHud
{
    internal sealed class MissingWindHudFieldException : ExtendedException
    {
        internal MissingWindHudFieldException(string fieldName, string objectName)
            : base("wind-hud-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }
}
