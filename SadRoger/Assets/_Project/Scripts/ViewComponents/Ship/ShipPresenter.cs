using System;
using Core.Gameplay.Navigation;
using Core.Gameplay.Ship;
using Core.Lifecycle;
using R3;

namespace ViewComponents.Ship
{
    public sealed class ShipPresenter : ISubscriptionLifecycle
    {
        private readonly IShipView _view;
        private readonly IReadOnlyShipModel _model;

        private IDisposable _subscriptions;

        public ShipPresenter(IShipView view, IReadOnlyShipModel model)
        {
            _view = view;
            _model = model;
        }

        void ISubscriptionLifecycle.Start()
        {
            _subscriptions = Disposable.Combine(
                _model.Position.Subscribe(OnPositionChanged),
                _model.Heading.Subscribe(_view.SetHeading)
            );
        }

        void ISubscriptionLifecycle.Stop()
        {
            _subscriptions?.Dispose();
        }

        private void OnPositionChanged(SeaPosition position)
        {
            _view.SetPosition(position.X, position.Z);
        }
    }
}
