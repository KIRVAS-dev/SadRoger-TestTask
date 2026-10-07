using Infrastructure.ExtendedExceptions;

namespace Core.Gameplay.GameFlow
{
    public sealed class GameStateMachine : IGameStateMachine
    {
        private readonly GameStateModel _model;

        public GameStateMachine(GameStateModel model)
        {
            _model = model;
        }

        void IGameStateMachine.EnterState(GameState state)
        {
            Guard.AgainstTrue(
                !IsTransitionAllowed(state),
                () => new InvalidGameStateTransitionException(state.ToString(), _model.State.Value)
            );

            _model.State.Value = state;
        }

        private bool IsTransitionAllowed(GameState state)
        {
            return state switch
            {
                GameState.Ready => _model.State.Value != GameState.Playing,
                GameState.Playing => _model.State.Value == GameState.Ready,
                GameState.Win or GameState.Lose => _model.State.Value == GameState.Playing,
                _ => throw new UnhandledGameStateException(state)
            };
        }
    }
}
