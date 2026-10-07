using ContentValidation;
using Core.Gameplay.GameFlow;
using Core.Gameplay.Ship;
using Core.Input;
using Core.Input.Ship;
using Core.Lifecycle;
using Core.Loop;
using Infrastructure.ExtendedExceptions;
using Input;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using ViewComponents.Audio;
using ViewComponents.Ship;

namespace Core.Bootstrap
{
    internal sealed class CoreScope : LifetimeScope
    {
        [SerializeField] private ShipConfig _shipConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterEntryPoint(builder);
            RegisterGameFlow(builder);
            RegisterAudioListenerFollow(builder);
            RegisterShip(builder);
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

        private void RegisterShip(IContainerBuilder builder)
        {
            Guard.AgainstNull(_shipConfig, () => Missing(nameof(_shipConfig)));

            builder.RegisterInstance(_shipConfig).As<IShipSettings>().As<IValidatable>();
            builder.Register<ShipKeyboardInput>(Lifetime.Singleton).As<IShipControlInput>().As<IInputTickable>();
            builder.Register<ShipModel>(Lifetime.Singleton).AsSelf().As<IReadOnlyShipModel>();
            builder.Register<ShipService>(Lifetime.Singleton).As<IShipService>().As<IGameplayTickable>();
            builder.Register<ShipInputHandler>(Lifetime.Singleton).As<ISubscriptionLifecycle>();

            builder.RegisterComponentInHierarchy<ShipView>().As<IShipView>();
            builder.Register<ShipPresenter>(Lifetime.Singleton).As<ISubscriptionLifecycle>();
        }

        private ExtendedException Missing(string fieldName)
        {
            return new MissingCoreScopeFieldException(fieldName, gameObject.name);
        }
    }
}
