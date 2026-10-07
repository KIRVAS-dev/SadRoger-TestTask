using Infrastructure.ExtendedExceptions;

namespace Core.Gameplay.Wind
{
    internal sealed class InvalidWindStrengthException : ExtendedException
    {
        internal InvalidWindStrengthException(float value)
            : base("wind-1", $"Wind strength '{value}' is outside [0, 1]") { }
    }

    internal sealed class InvalidWindMultiplierException : ExtendedException
    {
        internal InvalidWindMultiplierException(float value)
            : base("wind-2", $"Wind strength multiplier '{value}' is negative") { }
    }
}
