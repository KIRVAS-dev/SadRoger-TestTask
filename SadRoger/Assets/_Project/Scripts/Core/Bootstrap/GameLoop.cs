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
        private readonly IReadOnlyList<IPresentationTickable> _presentationTickables;

        public GameLoop(
            IReadOnlyList<IInputTickable> inputTickables,
            IReadOnlyList<IGameplayTickable> gameplayTickables,
            IReadOnlyList<IPresentationTickable> presentationTickables)
        {
            _inputTickables = inputTickables;
            _gameplayTickables = gameplayTickables;
            _presentationTickables = presentationTickables;
        }

        void ITickable.Tick()
        {
            float deltaTime = Time.deltaTime;

            UpdateInput();
            UpdateGameplay(deltaTime);
            UpdatePresentation();
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

        private void UpdatePresentation()
        {
            foreach (IPresentationTickable presentationTickable in _presentationTickables)
            {
                presentationTickable.Tick();
            }
        }
    }
}
