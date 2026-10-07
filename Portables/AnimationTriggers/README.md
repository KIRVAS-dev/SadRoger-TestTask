# AnimationTriggers

Trigger zone that plays an `Animator` state once, when an object first enters it.

## Dependencies

Base template only.

## Install

Copy `Scripts/` into `<UnityProject>/Assets/_Project/Scripts/` together with `.meta`. Folders mirror the project tree, so files land in the existing assemblies. Uninstall: delete the files listed below.

## Setup

- Add `AnimationTriggerZone` with its trigger collider; set the target `Animator` and the state name to play
- Validation: inside a level prefab — the Levels `LevelLoader` validates it; placed in a scene — register `.As<IValidatable>()` in `CoreScope`; Editor check — `Tools/ContentValidation`

## Files

- `ViewComponents/AnimationTriggers/AnimationTriggerZone.cs`
- `ViewComponents/AnimationTriggers/Api/Exceptions.cs`
