using Infrastructure.ExtendedExceptions;

namespace ViewComponents.AnimationTriggers
{
    internal sealed class MissingAnimationTriggerZoneFieldException : ExtendedException
    {
        internal MissingAnimationTriggerZoneFieldException(string fieldName, string objectName)
            : base("animation-trigger-1", $"Missing field {fieldName} on {objectName}") { }
    }

    internal sealed class InvalidAnimationTriggerZoneColliderException : ExtendedException
    {
        internal InvalidAnimationTriggerZoneColliderException(string objectName)
            : base("animation-trigger-2", $"Collider on {objectName} must have isTrigger enabled") { }
    }
}
