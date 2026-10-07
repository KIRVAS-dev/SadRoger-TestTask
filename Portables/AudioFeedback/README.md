# AudioFeedback

One-shot FMOD sounds by semantic type (`AudioFeedbackType` → `EventReference` table). No presenter — the game decides when to play.

## Dependencies

Base template only (FMOD).

## Install

Copy `Scripts/` into `<UnityProject>/Assets/_Project/Scripts/` together with `.meta`. Folders mirror the project tree, so files land in the existing assemblies. Uninstall: delete the files listed below.

## Setup

```csharp
// CoreScope
builder.RegisterComponentInHierarchy<AudioFeedbackPerformer>()
   .As<IAudioFeedbackPerformer>()
   .As<IValidatable>()
   .As<IWarmupLifecycle>();
```

- Replace the `ButtonClick` placeholder in `AudioFeedbackType` with the game's sound types (explicit int values); every type needs an entry
- Core scene: `AudioFeedbackPerformer` object in the gameplay group

## Files

- `Core/Gameplay/Feedback/Api/AudioFeedbackType.cs`
- `Core/Gameplay/Feedback/Api/IAudioFeedbackPerformer.cs`
- `ViewComponents/Feedback/Api/AudioFeedbackEntry.cs`
- `ViewComponents/Feedback/Api/Exceptions.cs`
- `ViewComponents/Feedback/AudioFeedbackPerformer.cs`
