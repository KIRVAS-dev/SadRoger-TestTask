using System;
using Core.Gameplay.Navigation;
using Infrastructure.ExtendedExceptions;

namespace Core.Gameplay.Wind
{
    public sealed class WindService
        : IWindService,
          IWindStrengthMultiplier
    {
        private const float MinStrength = 0f;
        private const float MaxStrength = 1f;
        private const float DefaultMultiplier = 1f;

        private readonly WindModel _model;

        private float _multiplier = DefaultMultiplier;

        public WindService(WindModel model)
        {
            _model = model;
        }

        void IWindService.SetDirection(float direction)
        {
            _model.Direction.Value = AngleHelper.NormalizeDegrees(direction);
        }

        void IWindService.SetStrength(float strength)
        {
            Guard.AgainstTrue(strength is < MinStrength or > MaxStrength, () => new InvalidWindStrengthException(strength));

            _model.BaseStrength.Value = strength;
            UpdateStrength();
        }

        void IWindStrengthMultiplier.SetMultiplier(float multiplier)
        {
            Guard.AgainstNegative(multiplier, () => new InvalidWindMultiplierException(multiplier));

            _multiplier = multiplier;
            UpdateStrength();
        }

        private void UpdateStrength()
        {
            _model.Strength.Value = Math.Clamp(_model.BaseStrength.Value * _multiplier, MinStrength, MaxStrength);
        }
    }
}
