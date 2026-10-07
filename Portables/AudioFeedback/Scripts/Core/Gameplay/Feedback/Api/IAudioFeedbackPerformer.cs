namespace Core.Gameplay.Feedback
{
    public interface IAudioFeedbackPerformer
    {
        void Play(AudioFeedbackType type);
    }
}
