using System;
using Core.Gameplay.GameFlow;
using Core.Loading;
using VContainer.Unity;

namespace Core.Bootstrap
{
    internal sealed class CoreEntryPoint
        : IStartable,
          IDisposable
    {
        private readonly IGameFlowService _gameFlowService;
        private readonly ILoadingService _loadingService;
        private readonly ScopeLifecycle _scopeLifecycle;

        public CoreEntryPoint(
            IGameFlowService gameFlowService,
            ILoadingService loadingService,
            ScopeLifecycle scopeLifecycle)
        {
            _gameFlowService = gameFlowService;
            _loadingService = loadingService;
            _scopeLifecycle = scopeLifecycle;
        }

        void IStartable.Start()
        {
            _scopeLifecycle.Start();
            _gameFlowService.PrepareGame();
            _loadingService.Complete();
        }

        void IDisposable.Dispose()
        {
            _scopeLifecycle.Stop();
        }
    }
}
