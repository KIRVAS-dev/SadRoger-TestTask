using Core.Gameplay.Ship;
using Core.Lifecycle;

namespace Core.Input.Ship
{
    public sealed class ShipInputHandler : ISubscriptionLifecycle
    {
        private readonly IShipControlInput _input;
        private readonly IShipService _shipService;

        public ShipInputHandler(IShipControlInput input, IShipService shipService)
        {
            _input = input;
            _shipService = shipService;
        }

        void ISubscriptionLifecycle.Start()
        {
            _input.ControlChanged += OnControlChanged;
        }

        void ISubscriptionLifecycle.Stop()
        {
            _input.ControlChanged -= OnControlChanged;
        }

        private void OnControlChanged()
        {
            _shipService.SetControl(_input.Throttle, _input.Steering);
        }
    }
}
