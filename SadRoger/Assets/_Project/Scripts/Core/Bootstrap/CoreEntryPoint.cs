using System;
using Core.Loading;
using VContainer.Unity;

namespace Core.Bootstrap
{
    internal sealed class CoreEntryPoint
        : IStartable,
          IDisposable
    {
        private readonly ILoadingService _loadingService;
        private readonly ScopeLifecycle _scopeLifecycle;

        public CoreEntryPoint(ILoadingService loadingService, ScopeLifecycle scopeLifecycle)
        {
            _loadingService = loadingService;
            _scopeLifecycle = scopeLifecycle;
        }

        void IStartable.Start()
        {
            _scopeLifecycle.Start();
            _loadingService.Complete();
        }

        void IDisposable.Dispose()
        {
            _scopeLifecycle.Stop();
        }
    }
}
