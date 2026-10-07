# TransformRotators

Rotates transforms at a constant speed while their renderer is visible, all in one `IPresentationTickable` tick.

## Dependencies

Base template only. The game implements `ITransformRotatorsRegistry`.

## Install

Copy `Scripts/` into `<UnityProject>/Assets/_Project/Scripts/` together with `.meta`. Folders mirror the project tree, so files land in the existing assemblies. Uninstall: delete the files listed below.

## Setup

```csharp
// CoreScope
builder.Register<TransformRotatorsTicker>(Lifetime.Singleton).As<IPresentationTickable>();
```

- Implement `ITransformRotatorsRegistry` where the game collects level or scene content (`GetComponentsInChildren<ITransformRotatorView>()`), register it `.As<ITransformRotatorsRegistry>()`
- Add `TransformRotatorWhileRendererVisible`; assign the visibility renderer and the target transform
- Validation: inside a level prefab — the Levels `LevelLoader` validates it; placed in a scene — register `.As<IValidatable>()` in `CoreScope`; Editor check — `Tools/ContentValidation`

## Files

- `Core/Gameplay/TransformRotator/Api/ITransformRotatorView.cs`
- `Core/Gameplay/TransformRotator/Api/ITransformRotatorsRegistry.cs`
- `ViewComponents/TransformRotators/Api/Exceptions.cs`
- `ViewComponents/TransformRotators/Api/RotationDirection.cs`
- `ViewComponents/TransformRotators/TransformRotatorWhileRendererVisible.cs`
- `ViewComponents/TransformRotators/TransformRotatorsTicker.cs`
