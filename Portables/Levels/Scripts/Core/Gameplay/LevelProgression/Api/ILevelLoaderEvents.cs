using System;

namespace Core.Gameplay.LevelProgression
{
    public interface ILevelLoaderEvents
    {
        event Action LevelLoaded;
    }
}
