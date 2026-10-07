using Core.Gameplay.GameFlow;
using VContainer;
using VContainer.Unity;
using ViewComponents.Audio;

namespace Core.Bootstrap
{
    internal sealed class CoreScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterEntryPoint(builder);
            RegisterGameFlow(builder);
            RegisterAudioListenerFollow(builder);
        }

        private static void RegisterEntryPoint(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<CoreEntryPoint>();
            builder.RegisterEntryPoint<GameLoop>();
            builder.Register<ScopeLifecycle>(Lifetime.Singleton);
        }

        private static void RegisterGameFlow(IContainerBuilder builder)
        {
            builder.Register<GameStateModel>(Lifetime.Singleton).AsSelf().As<IReadOnlyGameStateModel>();
            builder.Register<GameplayInputBlock>(Lifetime.Singleton).As<IGameplayInputBlock>();
            builder.Register<GameStateMachine>(Lifetime.Singleton).As<IGameStateMachine>();
            builder.Register<GameFlowService>(Lifetime.Singleton).As<IGameFlowService>();
        }

        private static void RegisterAudioListenerFollow(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<AudioListenerCameraFollower>();
        }
    }
}
