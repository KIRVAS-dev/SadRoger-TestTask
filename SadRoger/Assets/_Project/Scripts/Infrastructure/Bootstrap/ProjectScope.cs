using ContentValidation;
using Core.Bootstrap;
using Core.Bootstrap.Scene;
using Core.Lifecycle;
using Core.Loading;
using Infrastructure.Audio;
using Infrastructure.ExtendedExceptions;
using UI.LoadingScreen;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using ViewComponents.Audio;

namespace Infrastructure.Bootstrap
{
    internal sealed class ProjectScope : LifetimeScope
    {
        [SerializeField] private Transform _studioListener;
        [SerializeField] private LoadingScreenView _loadingScreenView;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterEntryPoint(builder);
            RegisterSceneLoading(builder);
            RegisterAudio(builder);
            RegisterLoading(builder);
            RegisterLoadingScreen(builder);
        }

        private static void RegisterEntryPoint(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<EntryPoint>();
            builder.Register<ScopeLifecycle>(Lifetime.Singleton);
        }

        private static void RegisterSceneLoading(IContainerBuilder builder)
        {
            builder.Register<CoreLoader>(Lifetime.Singleton).As<ISceneLoader>();
        }

        private static void RegisterLoading(IContainerBuilder builder)
        {
            builder.Register<LoadingModel>(Lifetime.Singleton).AsSelf().As<IReadOnlyLoadingModel>();
            builder.Register<LoadingService>(Lifetime.Singleton).As<ILoadingService>();
        }

        private void RegisterAudio(IContainerBuilder builder)
        {
            Guard.AgainstNull(_studioListener, () => new MissingStudioListenerAnchorTransformException());

            builder.RegisterInstance(new StudioListenerAnchor(_studioListener)).As<IStudioListenerAnchor>();
            builder.Register<FmodAudioLoader>(Lifetime.Singleton).As<IAudioLoader>();
        }

        private void RegisterLoadingScreen(IContainerBuilder builder)
        {
            Guard.AgainstNull(
                _loadingScreenView,
                () => new MissingLoadingScreenViewException(nameof(_loadingScreenView), gameObject.name)
            );

            builder.RegisterComponent(_loadingScreenView).As<ILoadingScreenView>().As<IValidatable>();
            builder.Register<LoadingScreenPresenter>(Lifetime.Singleton).As<ISubscriptionLifecycle>();
        }
    }
}
