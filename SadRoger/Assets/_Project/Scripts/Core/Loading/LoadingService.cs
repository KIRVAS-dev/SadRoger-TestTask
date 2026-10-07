using Infrastructure.ExtendedExceptions;

namespace Core.Loading
{
    public sealed class LoadingService : ILoadingService
    {
        private const float CompletedProgress = 1f;

        private readonly LoadingModel _model;

        public LoadingService(LoadingModel model)
        {
            _model = model;
        }

        void ILoadingService.SetProgress(float progress)
        {
            Guard.AgainstTrue(!_model.IsLoading.Value, () => new InvalidLoadingCompletionException());
            Guard.AgainstLessThan(progress, _model.Progress.Value, () => new InvalidLoadingProgressException(progress));
            Guard.AgainstGreaterThan(progress, CompletedProgress, () => new InvalidLoadingProgressException(progress));

            _model.Progress.Value = progress;
        }

        void ILoadingService.Complete()
        {
            Guard.AgainstTrue(!_model.IsLoading.Value, () => new InvalidLoadingCompletionException());

            _model.Progress.Value = CompletedProgress;
            _model.IsLoading.Value = false;
        }
    }
}
