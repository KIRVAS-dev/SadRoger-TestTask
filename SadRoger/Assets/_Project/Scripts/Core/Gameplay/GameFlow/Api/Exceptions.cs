using Infrastructure.ExtendedExceptions;

namespace Core.Gameplay.GameFlow
{
    internal sealed class InvalidGameStateTransitionException : ExtendedException
    {
        internal InvalidGameStateTransitionException(string attemptedTransition, GameState currentState)
            : base("game-flow-1", $"Transition '{attemptedTransition}' is not valid from state '{currentState}'") { }
    }
    internal sealed class UnhandledGameStateException : ExtendedException
    {
        internal UnhandledGameStateException(GameState state)
            : base("game-flow-2", $"GameState '{state}' is not handled by the game state machine") { }
    }
}
