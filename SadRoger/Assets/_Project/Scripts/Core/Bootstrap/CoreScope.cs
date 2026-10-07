using ContentValidation;
using Core.Gameplay.LocationBoundary;
using Core.Gameplay.Ship;
using Core.Gameplay.Weather;
using Core.Gameplay.Wind;
using Core.Input;
using Core.Input.Ship;
using Core.Input.Weather;
using Core.Input.Wind;
using Core.Lifecycle;
using Core.Loop;
using Infrastructure.ExtendedExceptions;
using Input;
using UI.BoundaryWarning;
using UI.LocationExitNotice;
using UI.WeatherHud;
using UI.WindHud;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using ViewComponents.LocationBoundary;
using ViewComponents.Ship;
using ViewComponents.Weather;
using ViewComponents.Wind;

namespace Core.Bootstrap
{
    internal sealed class CoreScope : LifetimeScope
    {
        [SerializeField] private ShipConfig _shipConfig;
        [SerializeField] private LocationBoundaryConfig _locationBoundaryConfig;
        [SerializeField] private WeatherConfig _weatherConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterEntryPoint(builder);
            RegisterShip(builder);
            RegisterWind(builder);
            RegisterWeather(builder);
            RegisterLocationBoundary(builder);
        }

        private static void RegisterEntryPoint(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<CoreEntryPoint>();
            builder.RegisterEntryPoint<GameLoop>();
            builder.Register<ScopeLifecycle>(Lifetime.Singleton);
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

        private static void RegisterWind(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<WindInspectorInput>().As<IWindInput>().As<IInputTickable>();
            builder.Register<WindModel>(Lifetime.Singleton).AsSelf().As<IReadOnlyWindModel>();
            builder.Register<WindService>(Lifetime.Singleton).As<IWindService>().As<IWindStrengthMultiplier>();
            builder.Register<RelativeWindAngle>(Lifetime.Singleton).As<IRelativeWindAngle>();
            builder.Register<WindInputHandler>(Lifetime.Singleton).As<ISubscriptionLifecycle>();

            builder.RegisterComponentInHierarchy<WindHudView>().As<IWindHudView>().As<IValidatable>().As<IPreparationLifecycle>();
            builder.Register<WindHudPresenter>(Lifetime.Singleton).As<ISubscriptionLifecycle>();
        }

        private void RegisterWeather(IContainerBuilder builder)
        {
            Guard.AgainstNull(_weatherConfig, () => Missing(nameof(_weatherConfig)));

            builder.RegisterInstance(_weatherConfig).As<IWeatherSettings>().As<IValidatable>();
            builder.RegisterComponentInHierarchy<WeatherInspectorInput>().As<IWeatherInput>().As<IInputTickable>();
            builder.Register<WeatherModel>(Lifetime.Singleton).AsSelf().As<IReadOnlyWeatherModel>();
            builder.Register<WeatherService>(Lifetime.Singleton).As<IWeatherService>().As<IPreparationLifecycle>();
            builder.Register<WeatherInputHandler>(Lifetime.Singleton).As<ISubscriptionLifecycle>();

            builder.RegisterComponentInHierarchy<WeatherFogView>().As<IWeatherFogView>();
            builder.Register<WeatherFogPresenter>(Lifetime.Singleton).As<ISubscriptionLifecycle>();

            builder
               .RegisterComponentInHierarchy<WeatherHudView>()
               .As<IWeatherHudView>()
               .As<IValidatable>()
               .As<IPreparationLifecycle>();

            builder.Register<WeatherHudPresenter>(Lifetime.Singleton).As<ISubscriptionLifecycle>();
        }

        private void RegisterLocationBoundary(IContainerBuilder builder)
        {
            Guard.AgainstNull(_locationBoundaryConfig, () => Missing(nameof(_locationBoundaryConfig)));

            builder.RegisterInstance(_locationBoundaryConfig).As<ILocationBoundarySettings>().As<IValidatable>();
            builder.RegisterComponentInHierarchy<LocationBoundaryCenter>().As<ILocationBoundaryCenterProvider>();
            builder.Register<LocationBoundaryModel>(Lifetime.Singleton).AsSelf().As<IReadOnlyLocationBoundaryModel>();

            builder.Register<LocationBoundaryService>(Lifetime.Singleton).As<ILocationBoundaryEvents>().As<IGameplayTickable>();

            builder.Register<LocationExitLogger>(Lifetime.Singleton).As<ISubscriptionLifecycle>();
            builder.RegisterComponentInHierarchy<LocationBoundaryRing>().As<IValidatable>().As<IPreparationLifecycle>();

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
