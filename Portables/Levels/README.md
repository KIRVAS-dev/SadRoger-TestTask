# Levels

Level progression: level list config, loader that spawns the level prefab, saved progress (PlayerPrefs), random level after the list is completed.

## Dependencies

Base template only. `Infrastructure.Persistence` is a new assembly: add it to the `Bootstrap.asmdef` references.

## Install

Copy `Scripts/` into `<UnityProject>/Assets/_Project/Scripts/` together with `.meta`. Folders mirror the project tree, so files land in the existing assemblies. Uninstall: delete the files listed below.

## Setup

```csharp
// CoreScope
builder.RegisterComponentInHierarchy<LevelLoader>().As<ILevelLoader>().As<ILevelLoaderEvents>().As<IValidatable>();
builder.Register<CurrentLevel>(Lifetime.Singleton);
builder.Register<PlayerPrefsLevelProgressStore>(Lifetime.Singleton).As<ILevelProgressStore>();
builder.Register<LevelModel>(Lifetime.Singleton);
builder.Register<LevelService>(Lifetime.Singleton).As<ILevelService>().As<ILevelProgress>();
```

- Core scene: `LevelLoader` object in the gameplay group, with a `LevelListConfig` (`Configs/LevelContent/`)
- Level prefab root: `Level` component; content as nested prefabs
- On load `LevelLoader` validates every `IValidatable` inside the level prefab
- Game content: read `CurrentLevel.Level` after `ILevelLoaderEvents.LevelLoaded`

## Files

- `Core/Gameplay/LevelProgression/Api/Exceptions.cs`
- `Core/Gameplay/LevelProgression/Api/ILevelLoader.cs`
- `Core/Gameplay/LevelProgression/Api/ILevelLoaderEvents.cs`
- `Core/Gameplay/LevelProgression/Api/ILevelProgress.cs`
- `Core/Gameplay/LevelProgression/Api/ILevelProgressStore.cs`
- `Core/Gameplay/LevelProgression/Api/ILevelService.cs`
- `Core/Gameplay/LevelProgression/LevelModel.cs`
- `Core/Gameplay/LevelProgression/LevelService.cs`
- `Infrastructure/Persistence/Infrastructure.Persistence.asmdef`
- `Infrastructure/Persistence/PlayerPrefsLevelProgressStore.cs`
- `ViewComponents/Level/Api/Exceptions.cs`
- `ViewComponents/Level/Api/LevelListConfig.cs`
- `ViewComponents/Level/CurrentLevel.cs`
- `ViewComponents/Level/Level.cs`
- `ViewComponents/Level/LevelLoader.cs`
