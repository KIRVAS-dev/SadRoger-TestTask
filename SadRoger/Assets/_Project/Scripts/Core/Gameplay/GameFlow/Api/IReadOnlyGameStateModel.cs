using R3;

namespace Core.Gameplay.GameFlow
{
    public interface IReadOnlyGameStateModel
    {
        ReadOnlyReactiveProperty<GameState> State { get; }
    }
}
