using Infrastructure.ExtendedExceptions;

namespace Core.Gameplay.LevelProgression
{
    internal sealed class InvalidLevelCountException : ExtendedException
    {
        internal InvalidLevelCountException(int levelCount)
            : base("level-1", $"Level count from provider must be positive, got {levelCount}") { }
    }
}
