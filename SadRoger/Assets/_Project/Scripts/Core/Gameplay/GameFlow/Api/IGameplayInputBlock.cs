using R3;

namespace Core.Gameplay.GameFlow
{
    public interface IGameplayInputBlock
    {
        ReadOnlyReactiveProperty<bool> IsBlocked { get; }
    }
}
