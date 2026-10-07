using System;
using ContentValidation;
using Core.Gameplay.LevelProgression;
using Infrastructure.ExtendedExceptions;
using UnityEngine;
using VContainer;

namespace ViewComponents.Level
{
    public sealed class LevelLoader
        : MonoBehaviour,
          ILevelLoader,
          ILevelLoaderEvents,
          IValidatable
    {
        [SerializeField] private LevelListConfig _config;

        private CurrentLevel _currentLevel;

        public event Action LevelLoaded;

        int ILevelLoader.LevelCount => _config.Levels.Count;
        bool ILevelLoader.IsRandomized => _config.IsRandomized;

        [Inject]
        private void Construct(CurrentLevel currentLevel)
        {
            _currentLevel = currentLevel;
        }

        void IValidatable.Validate()
        {
            Guard.AgainstNull(_config, () => new MissingLevelListConfigException(nameof(_config), gameObject.name));

            _config.Validate();
        }

        void ILevelLoader.LoadLevel(int levelIndex)
        {
            Level levelPrefab = _config.Levels[levelIndex];

            ClearChildren();
            SpawnLevel(levelPrefab);
        }

        private static void ValidateLevelContent(Level level)
        {
            IValidatable[] validatables = level.GetComponentsInChildren<IValidatable>(true);

            foreach (IValidatable validatable in validatables)
            {
                validatable.Validate();
            }
        }

        private void ClearChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        private void SpawnLevel(Level levelPrefab)
        {
            Level level = Instantiate(levelPrefab, transform);
            ValidateLevelContent(level);
            _currentLevel.Set(level);

            LevelLoaded?.Invoke();
        }
    }
}
