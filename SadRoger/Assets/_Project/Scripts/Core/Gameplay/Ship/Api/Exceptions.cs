using Infrastructure.ExtendedExceptions;

namespace Core.Gameplay.Ship
{
    internal sealed class InvalidShipControlException : ExtendedException
    {
        internal InvalidShipControlException(string axisName, float value)
            : base("ship-1", $"Control axis '{axisName}' has value '{value}' outside [-1, 1]") { }
    }
}
