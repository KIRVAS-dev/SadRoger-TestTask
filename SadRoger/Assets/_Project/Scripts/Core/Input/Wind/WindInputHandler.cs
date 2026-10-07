using Core.Gameplay.Wind;
using Core.Lifecycle;

namespace Core.Input.Wind
{
    public sealed class WindInputHandler : ISubscriptionLifecycle
    {
        private readonly IWindInput _input;
        private readonly IWindService _windService;

        public WindInputHandler(IWindInput input, IWindService windService)
        {
            _input = input;
            _windService = windService;
        }

        void ISubscriptionLifecycle.Start()
        {
            ApplyDirection();
            ApplyStrength();

            _input.DirectionChanged += ApplyDirection;
            _input.StrengthChanged += ApplyStrength;
        }

        void ISubscriptionLifecycle.Stop()
        {
            _input.DirectionChanged -= ApplyDirection;
            _input.StrengthChanged -= ApplyStrength;
        }

        private void ApplyDirection()
        {
            _windService.SetDirection(_input.Direction);
        }

        private void ApplyStrength()
        {
            _windService.SetStrength(_input.Strength);
        }
    }
}
