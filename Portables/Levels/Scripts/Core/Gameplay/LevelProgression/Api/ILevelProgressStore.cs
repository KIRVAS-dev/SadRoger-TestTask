namespace Core.Gameplay.LevelProgression
{
    public interface ILevelProgressStore
    {
        int LoadCompletedLevelCount();
        int LoadCurrentLevelIndex();

        void SaveProgress(int completedLevelCount, int currentLevelIndex);
    }
}
