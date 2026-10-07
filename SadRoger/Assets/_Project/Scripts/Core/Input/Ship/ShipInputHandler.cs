using System;
using Core.Gameplay.GameFlow;
using Core.Gameplay.Ship;
using Core.Lifecycle;
using R3;

namespace Core.Input.Ship
{
    public sealed class ShipInputHandler : ISubscriptionLifecycle
    {
        private readonly IShipControlInput _input;
        private readonly IShipService _shipService;
        private readonly IGameplayInputBlock _inputBlock;

        private IDisposable _blockSubscription;

        public ShipInputHandler(
            IShipControlInput input,
            IShipService shipService,
            IGameplayInputBlock inputBlock)
        {
            _input = input;
            _shipService = shipService;
            _inputBlock = inputBlock;
        }

        void ISubscriptionLifecycle.Start()
        {
            _input.ControlChanged += OnControlChanged;
            _blockSubscription = _inputBlock.IsBlocked.Subscribe(OnBlockChanged);
        }

        void ISubscriptionLifecycle.Stop()
        {
            _input.ControlChanged -= OnControlChanged;
            _blockSubscription?.Dispose();
        }

        private void OnControlChanged()
        {
            ApplyControl(_inputBlock.IsBlocked.CurrentValue);
        }

        private void OnBlockChanged(bool isBlocked)
        {
            ApplyControl(isBlocked);
        }

        private void ApplyControl(bool isBlocked)
        {
            if (isBlocked)
            {
                _shipService.SetControl(throttle: 0f, steering: 0f);
                return;
            }

            _shipService.SetControl(_input.Throttle, _input.Steering);
        }
    }
}
