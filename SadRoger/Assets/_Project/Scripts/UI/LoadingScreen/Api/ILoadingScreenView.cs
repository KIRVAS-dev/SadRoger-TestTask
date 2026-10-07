namespace UI.LoadingScreen
{
    public interface ILoadingScreenView
    {
        void Show();
        void Hide();
        void SetProgress(float progress);
    }
}
