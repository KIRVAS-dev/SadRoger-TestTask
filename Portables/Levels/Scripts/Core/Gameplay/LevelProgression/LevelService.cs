using Infrastructure.ExtendedExceptions;
using System;

namespace Core.Gameplay.LevelProgression
{
    public sealed class LevelService
        : ILevelService,
          ILevelProgress
    {
        private const int NoCompletedLevels = 0;
        private const int FirstLevelIndex = 0;

        private readonly ILevelLoader _levelLoader;
        private readonly ILevelProgressStore _progressStore;
        private readonly LevelModel _model;
        private readonly Random _random = new Random();

        public LevelService(
            ILevelLoader levelLoader,
            ILevelProgressStore progressStore,
            LevelModel model)
        {
            Guard.AgainstNonPositive(levelLoader.LevelCount, () => new InvalidLevelCountException(levelLoader.LevelCount));

            _levelLoader = levelLoader;
            _progressStore = progressStore;
            _model = model;

            _model.CompletedLevelCount = RestoredCompletedLevelCount(_progressStore.LoadCompletedLevelCount());

            _model.CurrentLevelIndex = RestoredLevelIndex(
                _progressStore.LoadCurrentLevelIndex(),
                _model.CompletedLevelCount,
                _levelLoader.LevelCount
            );
        }

        int ILevelProgress.CurrentLevelNumber => _model.CompletedLevelCount + 1;

        void ILevelService.LoadCurrentLevel()
        {
            _levelLoader.LoadLevel(_model.CurrentLevelIndex);
        }

        void ILevelService.LoadNextLevel()
        {
            _model.CompletedLevelCount++;
            _model.CurrentLevelIndex = NextLevelIndex();

            _progressStore.SaveProgress(_model.CompletedLevelCount, _model.CurrentLevelIndex);
            _levelLoader.LoadLevel(_model.CurrentLevelIndex);
        }

        private static int RestoredCompletedLevelCount(int savedCompletedLevelCount)
        {
            return Math.Max(savedCompletedLevelCount, NoCompletedLevels);
        }

        private static int RestoredLevelIndex(
            int savedLevelIndex,
            int completedLevelCount,
            int levelCount)
        {
            bool isSavedIndexInRange = savedLevelIndex >= FirstLevelIndex && savedLevelIndex < levelCount;

            return isSavedIndexInRange
                ? savedLevelIndex
                : SequentialLevelIndex(completedLevelCount, levelCount);
        }

        private static int SequentialLevelIndex(int completedLevelCount, int levelCount)
        {
            return completedLevelCount % levelCount;
        }

        private int NextLevelIndex()
        {
            int levelCount = _levelLoader.LevelCount;

            if (_model.CompletedLevelCount < levelCount)
            {
                return _model.CompletedLevelCount;
            }

            bool canPickRandomLevel = _levelLoader.IsRandomized && levelCount > 1;

            return canPickRandomLevel
                ? RandomLevelIndexExcludingCurrent(levelCount)
                : SequentialLevelIndex(_model.CompletedLevelCount, levelCount);
        }

        private int RandomLevelIndexExcludingCurrent(int levelCount)
        {
            int randomIndex = _random.Next(levelCount - 1);

            return randomIndex >= _model.CurrentLevelIndex
                ? randomIndex + 1
                : randomIndex;
        }
    }
}
