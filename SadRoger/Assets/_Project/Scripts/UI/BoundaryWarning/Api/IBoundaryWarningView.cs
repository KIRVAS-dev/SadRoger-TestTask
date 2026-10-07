namespace UI.BoundaryWarning
{
    public interface IBoundaryWarningView
    {
        void Show();
        void Hide();
        void SetSecondsLeft(int secondsLeft);
    }
}
