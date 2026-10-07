#if UNITY_EDITOR
using UnityEngine.InputSystem;

namespace ProjectDebug
{
    internal static class DebugHotkey
    {
        internal static bool WasPressedThisFrame(Key hotkey)
        {
            return Keyboard.current != null && Keyboard.current[hotkey].wasPressedThisFrame;
        }
    }
}
#endif
