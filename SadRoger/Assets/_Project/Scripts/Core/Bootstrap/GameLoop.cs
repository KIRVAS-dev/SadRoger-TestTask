using System.Collections.Generic;
using Core.Loop;
using UnityEngine;
using VContainer.Unity;

namespace Core.Bootstrap
{
    internal sealed class GameLoop : ITickable
    {
        private readonly IReadOnlyList<IInputTickable> _inputTickables;
        private readonly IReadOnlyList<IGameplayTickable> _gameplayTickables;

        public GameLoop(IReadOnlyList<IInputTickable> inputTickables, IReadOnlyList<IGameplayTickable> gameplayTickables)
        {
            _inputTickables = inputTickables;
            _gameplayTickables = gameplayTickables;
        }

        void ITickable.Tick()
        {
            float deltaTime = Time.deltaTime;

            UpdateInput();
            UpdateGameplay(deltaTime);
        }

        private void UpdateInput()
        {
            foreach (IInputTickable inputTickable in _inputTickables)
            {
                inputTickable.Tick();
            }
        }

        private void UpdateGameplay(float deltaTime)
        {
            foreach (IGameplayTickable gameplayTickable in _gameplayTickables)
            {
                gameplayTickable.Tick(deltaTime);
            }
        }
    }
}
