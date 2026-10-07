using System;
using R3;

namespace Core.Gameplay.GameFlow
{
    public sealed class GameplayInputBlock
        : IGameplayInputBlock,
          IDisposable
    {
        private readonly ReadOnlyReactiveProperty<bool> _isBlocked;

        public GameplayInputBlock(IReadOnlyGameStateModel model)
        {
            _isBlocked = model.State.Select(state => state != GameState.Playing).ToReadOnlyReactiveProperty();
        }

        ReadOnlyReactiveProperty<bool> IGameplayInputBlock.IsBlocked => _isBlocked;

        void IDisposable.Dispose()
        {
            _isBlocked.Dispose();
        }
    }
}
