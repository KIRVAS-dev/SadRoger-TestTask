using Core.Gameplay.Navigation;
using Core.Loop;
using Infrastructure.ExtendedExceptions;

namespace Core.Gameplay.Ship
{
    public sealed class ShipService
        : IShipService,
          IGameplayTickable
    {
        private const float MinAxis = -1f;
        private const float MaxAxis = 1f;

        private readonly IShipSettings _settings;
        private readonly ShipModel _model;

        private float _throttle;
        private float _steering;

        public ShipService(IShipSettings settings, ShipModel model)
        {
            _settings = settings;
            _model = model;
        }

        void IShipService.SetControl(float throttle, float steering)
        {
            Guard.AgainstTrue(!IsAxisValid(throttle), () => Invalid(nameof(throttle), throttle));
            Guard.AgainstTrue(!IsAxisValid(steering), () => Invalid(nameof(steering), steering));

            _throttle = throttle;
            _steering = steering;

            return;

            ExtendedException Invalid(string axisName, float value) => new InvalidShipControlException(axisName, value);
        }

        void IGameplayTickable.Tick(float deltaTime)
        {
            RotateShip(deltaTime);
            MoveShip(deltaTime);
        }

        private static bool IsAxisValid(float value)
        {
            return value is >= MinAxis and <= MaxAxis;
        }

        private void RotateShip(float deltaTime)
        {
            if (_steering == 0f)
            {
                return;
            }

            float heading = _model.Heading.Value + _steering * _settings.TurnSpeed * deltaTime;
            _model.Heading.Value = AngleHelper.NormalizeDegrees(heading);
        }

        private void MoveShip(float deltaTime)
        {
            if (_throttle == 0f)
            {
                return;
            }

            float heading = _model.Heading.Value;
            float distance = _throttle * _settings.MoveSpeed * deltaTime;
            SeaPosition position = _model.Position.Value;

            _model.Position.Value = new SeaPosition(
                position.X + AngleHelper.ForwardX(heading) * distance,
                position.Z + AngleHelper.ForwardZ(heading) * distance
            );
        }
    }
}
