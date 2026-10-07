using Cysharp.Threading.Tasks;
using Infrastructure.ExtendedExceptions;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using CoreLoadSceneMode = Core.Bootstrap.Scene.LoadSceneMode;
using UnityLoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;
using UnityScene = UnityEngine.SceneManagement.Scene;

namespace Core.Bootstrap.Scene
{
    public sealed class CoreLoader : ISceneLoader
    {
        void ISceneLoader.Load(string sceneName, CoreLoadSceneMode loadSceneMode)
        {
            UnityLoadSceneMode unityLoadSceneMode = ConvertLoadSceneMode(loadSceneMode);

            SceneManager.LoadScene(sceneName, unityLoadSceneMode);
        }

        async UniTask ISceneLoader.LoadAsync(
            string sceneName,
            CoreLoadSceneMode loadSceneMode,
            IProgress<float> progress,
            CancellationToken cancellationToken)
        {
            UnityLoadSceneMode unityLoadSceneMode = ConvertLoadSceneMode(loadSceneMode);

            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, unityLoadSceneMode);

            Guard.AgainstNull(loadOperation, () => new SceneNotFoundException(sceneName));

            await loadOperation.ToUniTask(progress, cancellationToken: cancellationToken);
        }

        void ISceneLoader.SetActiveScene(string sceneName)
        {
            UnityScene scene = SceneManager.GetSceneByName(sceneName);

            Guard.AgainstTrue(!SceneManager.SetActiveScene(scene), () => new SceneActivationException(sceneName));
        }

        private static UnityLoadSceneMode ConvertLoadSceneMode(CoreLoadSceneMode loadSceneMode)
        {
            return loadSceneMode switch
            {
                CoreLoadSceneMode.Single => UnityLoadSceneMode.Single,
                CoreLoadSceneMode.Additive => UnityLoadSceneMode.Additive,
                _ => throw new UnhandledLoadSceneModeException(loadSceneMode)
            };
        }
    }
}
