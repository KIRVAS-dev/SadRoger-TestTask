using R3;

namespace Core.Loading
{
    public interface IReadOnlyLoadingModel
    {
        ReadOnlyReactiveProperty<bool> IsLoading { get; }
        ReadOnlyReactiveProperty<float> Progress { get; }
    }
}
