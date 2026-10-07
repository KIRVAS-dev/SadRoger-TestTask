using R3;

namespace Core.Gameplay.GameFlow
{
    public sealed class GameStateModel : IReadOnlyGameStateModel
    {
        public ReactiveProperty<GameState> State { get; } = new ReactiveProperty<GameState>(GameState.Ready);

        ReadOnlyReactiveProperty<GameState> IReadOnlyGameStateModel.State => State;
    }
}
