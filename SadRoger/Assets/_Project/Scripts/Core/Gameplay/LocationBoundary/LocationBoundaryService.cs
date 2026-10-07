using System;
using Core.Gameplay.Ship;
using Core.Loop;

namespace Core.Gameplay.LocationBoundary
{
    public sealed class LocationBoundaryService
        : ILocationBoundaryEvents,
          IGameplayTickable
    {
        private readonly ILocationBoundarySettings _settings;
        private readonly IReadOnlyShipModel _shipModel;
        private readonly LocationBoundaryModel _model;

        private float _outsideTime;

        public event Action LocationExited;

        public LocationBoundaryService(
            ILocationBoundarySettings settings,
            IReadOnlyShipModel shipModel,
            LocationBoundaryModel model)
        {
            _settings = settings;
            _shipModel = shipModel;
            _model = model;
        }

        void IGameplayTickable.Tick(float deltaTime)
        {
            if (!IsShipOutside())
            {
                ReturnInside();
                return;
            }

            switch (_model.State.Value)
            {
                case LocationBoundaryState.Inside:
                    StartLeaving();
                    break;

                case LocationBoundaryState.Leaving:
                case LocationBoundaryState.Warning:
                    AdvanceLeaving(deltaTime);
                    break;

                case LocationBoundaryState.Exited:
                    break;

                default:
                    throw new UnhandledLocationBoundaryStateException(_model.State.Value);
            }
        }

        private bool IsShipOutside()
        {
            float radius = _settings.Radius;

            return _shipModel.Position.CurrentValue.SquaredDistanceTo(_settings.CenterX, _settings.CenterZ) > radius * radius;
        }

        private void ReturnInside()
        {
            _outsideTime = 0f;
            _model.CountdownRemaining.Value = _settings.ExitCountdown;
            _model.State.Value = LocationBoundaryState.Inside;
        }

        private void StartLeaving()
        {
            _outsideTime = 0f;
            _model.CountdownRemaining.Value = _settings.ExitCountdown;
            _model.State.Value = LocationBoundaryState.Leaving;
        }

        private void AdvanceLeaving(float deltaTime)
        {
            _outsideTime += deltaTime;

            float countdownElapsed = _outsideTime - _settings.WarningDelay;

            if (countdownElapsed < 0f)
            {
                return;
            }

            if (countdownElapsed >= _settings.ExitCountdown)
            {
                ExitLocation();
                return;
            }

            _model.CountdownRemaining.Value = _settings.ExitCountdown - countdownElapsed;
            _model.State.Value = LocationBoundaryState.Warning;
        }

        private void ExitLocation()
        {
            _model.CountdownRemaining.Value = 0f;
            _model.State.Value = LocationBoundaryState.Exited;
            LocationExited?.Invoke();
        }
    }
}
