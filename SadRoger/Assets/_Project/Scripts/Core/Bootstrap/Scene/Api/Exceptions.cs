using Infrastructure.ExtendedExceptions;

namespace Core.Bootstrap.Scene
{
    internal sealed class UnhandledLoadSceneModeException : ExtendedException
    {
        internal UnhandledLoadSceneModeException(LoadSceneMode loadSceneMode)
            : base("bootstrap-scene-1", $"Unhandled load scene mode: {loadSceneMode}") { }
    }

    internal sealed class SceneNotFoundException : ExtendedException
    {
        internal SceneNotFoundException(string sceneName)
            : base("bootstrap-scene-2", $"Scene '{sceneName}' cannot be loaded, check Build Settings") { }
    }

    internal sealed class SceneActivationException : ExtendedException
    {
        internal SceneActivationException(string sceneName)
            : base("bootstrap-scene-3", $"Scene '{sceneName}' cannot be set active, it must be loaded") { }
    }
}
