namespace Core.Loading
{
    public interface ILoadingService
    {
        void SetProgress(float progress);
        void Complete();
    }
}
