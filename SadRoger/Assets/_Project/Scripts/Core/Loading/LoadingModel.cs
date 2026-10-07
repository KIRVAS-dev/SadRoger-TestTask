using R3;

namespace Core.Loading
{
    public sealed class LoadingModel : IReadOnlyLoadingModel
    {
        public ReactiveProperty<bool> IsLoading { get; } = new ReactiveProperty<bool>(true);
        public ReactiveProperty<float> Progress { get; } = new ReactiveProperty<float>(0f);

        ReadOnlyReactiveProperty<bool> IReadOnlyLoadingModel.IsLoading => IsLoading;
        ReadOnlyReactiveProperty<float> IReadOnlyLoadingModel.Progress => Progress;
    }
}
