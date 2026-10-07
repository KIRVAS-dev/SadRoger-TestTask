using System;
using System.Threading;
using Core.Bootstrap;
using Core.Bootstrap.Scene;
using Core.Loading;
using Cysharp.Threading.Tasks;
using Infrastructure.Audio;
using VContainer.Unity;

namespace Infrastructure.Bootstrap
{
    internal sealed class EntryPoint
        : IStartable,
          IDisposable
    {
        private const string CoreSceneName = "Core";
        private const float AudioLoadedProgress = 0.2f;
        private const float SceneLoadedProgress = 1f;

        private readonly IAudioLoader _audioLoader;
        private readonly ILoadingService _loadingService;
        private readonly ISceneLoader _sceneLoader;
        private readonly ScopeLifecycle _scopeLifecycle;

        public EntryPoint(
            IAudioLoader audioLoader,
            ILoadingService loadingService,
            ISceneLoader sceneLoader,
            ScopeLifecycle scopeLifecycle)
        {
            _audioLoader = audioLoader;
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
            await _audioLoader.LoadAsync(CancellationToken.None);

            _loadingService.SetProgress(AudioLoadedProgress);

            IProgress<float> sceneProgress = Progress.Create<float>(ReportSceneProgress);

            await _sceneLoader.LoadAsync(CoreSceneName, LoadSceneMode.Additive, sceneProgress, CancellationToken.None);

            _sceneLoader.SetActiveScene(CoreSceneName);
        }

        private void ReportSceneProgress(float sceneProgress)
        {
            float progress = AudioLoadedProgress + sceneProgress * (SceneLoadedProgress - AudioLoadedProgress);

            _loadingService.SetProgress(progress);
        }
    }
}
