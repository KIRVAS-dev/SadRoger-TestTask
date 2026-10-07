#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.InputSystem;

namespace ProjectDebug
{
    internal sealed class DebugPause : MonoBehaviour
    {
        [SerializeField] private Key _hotkey = Key.P;

        private float _timeScaleBeforePause;
        private bool _isPaused;

        private void Update()
        {
            if (!DebugHotkey.WasPressedThisFrame(_hotkey))
            {
                return;
            }

            if (_isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        private void OnDestroy()
        {
            if (_isPaused)
            {
                Resume();
            }
        }

        private void Pause()
        {
            _timeScaleBeforePause = Time.timeScale;
            Time.timeScale = 0f;
            _isPaused = true;
        }

        private void Resume()
        {
            Time.timeScale = _timeScaleBeforePause;
            _isPaused = false;
        }
    }
}
#endif
