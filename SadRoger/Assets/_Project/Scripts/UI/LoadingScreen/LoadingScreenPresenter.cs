using System;
using Core.Lifecycle;
using Core.Loading;
using R3;

namespace UI.LoadingScreen
{
    public sealed class LoadingScreenPresenter : ISubscriptionLifecycle
    {
        private readonly ILoadingScreenView _view;
        private readonly IReadOnlyLoadingModel _loadingModel;

        private IDisposable _subscriptions;

        public LoadingScreenPresenter(ILoadingScreenView view, IReadOnlyLoadingModel loadingModel)
        {
            _view = view;
            _loadingModel = loadingModel;
        }

        void ISubscriptionLifecycle.Start()
        {
            _subscriptions = Disposable.Combine(
                _loadingModel.IsLoading.Subscribe(OnLoadingChanged),
                _loadingModel.Progress.Subscribe(_view.SetProgress)
            );
        }

        void ISubscriptionLifecycle.Stop()
        {
            _subscriptions?.Dispose();
        }

        private void OnLoadingChanged(bool isLoading)
        {
            if (isLoading)
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
