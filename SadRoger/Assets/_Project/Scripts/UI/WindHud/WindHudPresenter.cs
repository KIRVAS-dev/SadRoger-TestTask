using System;
using Core.Gameplay.Wind;
using Core.Lifecycle;
using R3;

namespace UI.WindHud
{
    public sealed class WindHudPresenter : ISubscriptionLifecycle
    {
        private readonly IWindHudView _view;
        private readonly IReadOnlyWindModel _windModel;
        private readonly IRelativeWindAngle _relativeWindAngle;
        private readonly IWindService _windService;

        private IDisposable _subscriptions;

        public WindHudPresenter(
            IWindHudView view,
            IReadOnlyWindModel windModel,
            IRelativeWindAngle relativeWindAngle,
            IWindService windService)
        {
            _view = view;
            _windModel = windModel;
            _relativeWindAngle = relativeWindAngle;
            _windService = windService;
        }

        void ISubscriptionLifecycle.Start()
        {
            _subscriptions = Disposable.Combine(
                _windModel.Direction.Subscribe(_view.SetDirection),
                _windModel.BaseStrength.Subscribe(_view.SetBaseStrength),
                _windModel.Strength.Subscribe(_view.SetStrength),
                _relativeWindAngle.Angle.Subscribe(_view.SetRelativeAngle)
            );

            _view.DirectionChanged += OnDirectionChanged;
            _view.BaseStrengthChanged += OnBaseStrengthChanged;
        }

        void ISubscriptionLifecycle.Stop()
        {
            _view.DirectionChanged -= OnDirectionChanged;
            _view.BaseStrengthChanged -= OnBaseStrengthChanged;
            _subscriptions?.Dispose();
        }

        private void OnDirectionChanged(float direction)
        {
            _windService.SetDirection(direction);
        }

        private void OnBaseStrengthChanged(float baseStrength)
        {
            _windService.SetStrength(baseStrength);
        }
    }
}
