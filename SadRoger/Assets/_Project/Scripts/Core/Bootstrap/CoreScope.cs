using ContentValidation;
using Core.Gameplay.GameFlow;
using Core.Gameplay.LocationBoundary;
using Core.Gameplay.Ship;
using Core.Gameplay.Wind;
using Core.Input;
using Core.Input.Ship;
using Core.Input.Wind;
using Core.Lifecycle;
using Core.Loop;
using Infrastructure.ExtendedExceptions;
using Input;
using UI.BoundaryWarning;
using UI.LocationExitNotice;
using UI.WindHud;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using ViewComponents.Audio;
using ViewComponents.LocationBoundary;
using ViewComponents.Ship;
using ViewComponents.Wind;

namespace Core.Bootstrap
{
    internal sealed class CoreScope : LifetimeScope
    {
        [SerializeField] private ShipConfig _shipConfig;
        [SerializeField] private LocationBoundaryConfig _locationBoundaryConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterEntryPoint(builder);
            RegisterGameFlow(builder);
            RegisterAudioListenerFollow(builder);
            RegisterShip(builder);
            RegisterWind(builder);
            RegisterLocationBoundary(builder);
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

        private static void RegisterWind(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<WindInspectorInput>().As<IWindInput>().As<IInputTickable>();
            builder.Register<WindModel>(Lifetime.Singleton).AsSelf().As<IReadOnlyWindModel>();
            builder.Register<WindService>(Lifetime.Singleton).As<IWindService>().As<IWindStrengthMultiplier>();
            builder.Register<RelativeWindAngle>(Lifetime.Singleton).As<IRelativeWindAngle>();
            builder.Register<WindInputHandler>(Lifetime.Singleton).As<ISubscriptionLifecycle>();

            builder.RegisterComponentInHierarchy<WindHudView>().As<IWindHudView>().As<IValidatable>();
            builder.Register<WindHudPresenter>(Lifetime.Singleton).As<ISubscriptionLifecycle>();
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

        private void RegisterLocationBoundary(IContainerBuilder builder)
        {
            Guard.AgainstNull(_locationBoundaryConfig, () => Missing(nameof(_locationBoundaryConfig)));

            builder.RegisterInstance(_locationBoundaryConfig).As<ILocationBoundarySettings>().As<IValidatable>();
            builder.Register<LocationBoundaryModel>(Lifetime.Singleton).AsSelf().As<IReadOnlyLocationBoundaryModel>();

            builder.Register<LocationBoundaryService>(Lifetime.Singleton).As<ILocationBoundaryEvents>().As<IGameplayTickable>();

            builder.Register<LocationExitLogger>(Lifetime.Singleton).As<ISubscriptionLifecycle>();
            builder.RegisterComponentInHierarchy<LocationBoundaryRing>().As<IValidatable>().As<IWarmupLifecycle>();

            builder.RegisterComponentInHierarchy<BoundaryWarningView>().As<IBoundaryWarningView>().As<IValidatable>();
            builder.Register<BoundaryWarningPresenter>(Lifetime.Singleton).As<ISubscriptionLifecycle>();

            builder.RegisterComponentInHierarchy<LocationExitNoticeView>().As<ILocationExitNoticeView>().As<IValidatable>();

            builder.Register<LocationExitNoticePresenter>(Lifetime.Singleton).As<ISubscriptionLifecycle>();
        }

        private ExtendedException Missing(string fieldName)
        {
            return new MissingCoreScopeFieldException(fieldName, gameObject.name);
        }
    }
}
