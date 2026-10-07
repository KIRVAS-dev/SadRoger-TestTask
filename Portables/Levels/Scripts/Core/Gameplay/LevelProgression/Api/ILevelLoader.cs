namespace Core.Gameplay.LevelProgression
{
    public interface ILevelLoader
    {
        int LevelCount { get; }
        bool IsRandomized { get; }
        void LoadLevel(int levelIndex);
    }
}
