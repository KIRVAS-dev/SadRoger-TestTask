# AnimationSounds

FMOD sounds fired from animation events.

## Dependencies

Base template only (FMOD).

## Install

Copy `Scripts/` into `<UnityProject>/Assets/_Project/Scripts/` together with `.meta`. Folders mirror the project tree, so files land in the existing assemblies. Uninstall: delete the files listed below.

## Setup

- Add `AnimationSoundEmitter` next to the `Animator`; clip events call `PlaySound` with an `AnimationSoundConfig` asset (FMOD event) as the object parameter
- Validation: inside a level prefab — the Levels `LevelLoader` validates it; placed in a scene — register `.As<IValidatable>()` in `CoreScope`; Editor check — `Tools/ContentValidation`

## Files

- `ViewComponents/AnimationSounds/AnimationSoundEmitter.cs`
- `ViewComponents/AnimationSounds/Api/AnimationSoundConfig.cs`
- `ViewComponents/AnimationSounds/Api/Exceptions.cs`
