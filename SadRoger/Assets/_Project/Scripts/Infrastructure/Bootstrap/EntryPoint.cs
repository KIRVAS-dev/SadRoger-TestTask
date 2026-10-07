using System;
using System.Threading;
using Core.Bootstrap;
using Core.Bootstrap.Scene;
using Core.Loading;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Infrastructure.Bootstrap
{
    internal sealed class EntryPoint
        : IStartable,
          IDisposable
    {
        private const string CoreSceneName = "Core";

        private readonly ILoadingService _loadingService;
        private readonly ISceneLoader _sceneLoader;
        private readonly ScopeLifecycle _scopeLifecycle;

        public EntryPoint(
            ILoadingService loadingService,
            ISceneLoader sceneLoader,
            ScopeLifecycle scopeLifecycle)
        {
            _loadingService = loadingService;
            _sceneLoader = sceneLoader;
            _scopeLifecycle = scopeLifecycle;
        }

        void IStartable.Start()
        {
            _scopeLifecycle.Start();

            LoadCoreAsync().Forget();
        }

        void IDisposable.Dispose()
        {
            _scopeLifecycle.Stop();
        }

        private async UniTaskVoid LoadCoreAsync()
        {
            IProgress<float> sceneProgress = Progress.Create<float>(_loadingService.SetProgress);

            await _sceneLoader.LoadAsync(CoreSceneName, LoadSceneMode.Additive, sceneProgress, CancellationToken.None);

            _sceneLoader.SetActiveScene(CoreSceneName);
        }
    }
}
