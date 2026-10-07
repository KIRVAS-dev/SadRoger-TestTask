using Infrastructure.ExtendedExceptions;

namespace ViewComponents.Ship
{
    internal sealed class InvalidShipValueException : ExtendedException
    {
        internal InvalidShipValueException(string fieldName, float value)
            : base("ship-view-1", $"Field '{fieldName}' has invalid value '{value}'") { }
    }
}
