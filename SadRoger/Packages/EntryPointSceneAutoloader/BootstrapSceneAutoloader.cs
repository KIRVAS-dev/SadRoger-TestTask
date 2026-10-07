using UnityEditor;
using UnityEditor.SceneManagement;

namespace Kirvas.BootstrapSceneLoader
{
    [InitializeOnLoad]
    public static class BootstrapSceneAutoloader
    {
        const string MenuPath = "PlayFromBootstrap/Enabled";
        const string PlayFromBootstrapKey = "PlayFromBootstrapKey";
        const int BootSceneIndex = 0;

        static BootstrapSceneAutoloader()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        [MenuItem(MenuPath)]
        static void Toggle()
        {
            bool result = EditorPrefs.GetBool(PlayFromBootstrapKey);
            EditorPrefs.SetBool(PlayFromBootstrapKey, !result);
        }

        [MenuItem(MenuPath, isValidateFunction: true)]
        static bool ToggleValidate()
        {
            Menu.SetChecked(MenuPath, EditorPrefs.GetBool(PlayFromBootstrapKey));
            return true;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingEditMode)
            {
                return;
            }

            if (!EditorPrefs.GetBool(PlayFromBootstrapKey))
            {
                EditorSceneManager.playModeStartScene = null;
                return;
            }

            if (EditorBuildSettings.scenes.Length == 0)
            {
                return;
            }

            EditorSceneManager.playModeStartScene =
                AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[BootSceneIndex].path);
        }
    }
}
