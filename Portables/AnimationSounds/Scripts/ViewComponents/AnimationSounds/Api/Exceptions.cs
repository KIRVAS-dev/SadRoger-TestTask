using Infrastructure.ExtendedExceptions;

namespace ViewComponents.AnimationSounds
{
    internal sealed class MissingAnimationSoundEmitterFieldException : ExtendedException
    {
        internal MissingAnimationSoundEmitterFieldException(string fieldName, string objectName)
            : base("animation-sound-1", $"Field '{fieldName}' is not assigned on '{objectName}'") { }
    }

    internal sealed class MissingAnimationSoundConfigException : ExtendedException
    {
        internal MissingAnimationSoundConfigException(string clipName, string objectName)
            : base("animation-sound-2", $"PlaySound event in clip '{clipName}' has no AnimationSoundConfig on '{objectName}'") { }
    }

    internal sealed class MissingAnimationSoundException : ExtendedException
    {
        internal MissingAnimationSoundException(string configName)
            : base("animation-sound-3", $"Missing FMOD event in animation sound config '{configName}'") { }
    }

    internal sealed class MissingAnimationSoundControllerException : ExtendedException
    {
        internal MissingAnimationSoundControllerException(string objectName)
            : base("animation-sound-4", $"Animator on '{objectName}' has no runtime animator controller") { }
    }
}
