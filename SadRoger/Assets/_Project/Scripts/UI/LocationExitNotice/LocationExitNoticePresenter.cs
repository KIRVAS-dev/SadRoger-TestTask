using System;
using Core.Gameplay.LocationBoundary;
using Core.Lifecycle;
using R3;

namespace UI.LocationExitNotice
{
    public sealed class LocationExitNoticePresenter : ISubscriptionLifecycle
    {
        private readonly ILocationExitNoticeView _view;
        private readonly ILocationBoundaryEvents _events;
        private readonly IReadOnlyLocationBoundaryModel _model;

        private IDisposable _stateSubscription;

        public LocationExitNoticePresenter(
            ILocationExitNoticeView view,
            ILocationBoundaryEvents events,
            IReadOnlyLocationBoundaryModel model)
        {
            _view = view;
            _events = events;
            _model = model;
        }

        void ISubscriptionLifecycle.Start()
        {
            _events.LocationExited += OnLocationExited;
            _stateSubscription = _model.State.Subscribe(OnStateChanged);
        }

        void ISubscriptionLifecycle.Stop()
        {
            _events.LocationExited -= OnLocationExited;
            _stateSubscription?.Dispose();
        }

        private void OnLocationExited()
        {
            _view.Show();
        }

        private void OnStateChanged(LocationBoundaryState state)
        {
            if (state == LocationBoundaryState.Inside)
            {
                _view.Hide();
            }
        }
    }
}
