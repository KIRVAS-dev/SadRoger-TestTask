#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ProjectDebug
{
    internal sealed class DebugSceneRestart : MonoBehaviour
    {
        [SerializeField] private Key _hotkey = Key.R;
        [SerializeField] private SceneAsset _bootScene;

        private void Update()
        {
            if (!DebugHotkey.WasPressedThisFrame(_hotkey))
            {
                return;
            }

            string bootScenePath = AssetDatabase.GetAssetPath(_bootScene);
            EditorSceneManager.LoadSceneInPlayMode(bootScenePath, new LoadSceneParameters(LoadSceneMode.Single));
        }
    }
}
#endif
