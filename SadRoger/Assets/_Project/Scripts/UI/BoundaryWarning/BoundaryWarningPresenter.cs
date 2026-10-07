using System;
using Core.Gameplay.LocationBoundary;
using Core.Lifecycle;
using R3;

namespace UI.BoundaryWarning
{
    public sealed class BoundaryWarningPresenter : ISubscriptionLifecycle
    {
        private readonly IBoundaryWarningView _view;
        private readonly IReadOnlyLocationBoundaryModel _model;

        private IDisposable _subscriptions;

        public BoundaryWarningPresenter(IBoundaryWarningView view, IReadOnlyLocationBoundaryModel model)
        {
            _view = view;
            _model = model;
        }

        void ISubscriptionLifecycle.Start()
        {
            _subscriptions = Disposable.Combine(
                _model.State.Subscribe(OnStateChanged),
                _model.CountdownRemaining.Select(SecondsLeft).DistinctUntilChanged().Subscribe(_view.SetSecondsLeft)
            );
        }

        void ISubscriptionLifecycle.Stop()
        {
            _subscriptions?.Dispose();
        }

        private static int SecondsLeft(float countdownRemaining)
        {
            return (int)MathF.Ceiling(countdownRemaining);
        }

        private void OnStateChanged(LocationBoundaryState state)
        {
            if (state == LocationBoundaryState.Warning)
            {
                _view.Show();
            }
            else
            {
                _view.Hide();
            }
        }
    }
}
