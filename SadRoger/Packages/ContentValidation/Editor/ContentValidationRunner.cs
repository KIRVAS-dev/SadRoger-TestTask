using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ContentValidation.Editor
{
    public static class ContentValidationRunner
    {
        public static void ValidateAll()
        {
            List<string> errors = new List<string>();

            ValidateContentAssets(errors);
            ValidateOpenScenes(errors);

            Report(errors, "Validate All");
        }

        public static void ValidateContentAssets()
        {
            List<string> errors = new List<string>();

            ValidateContentAssets(errors);

            Report(errors, "Validate Content Assets");
        }

        public static void ValidateOpenScenes()
        {
            List<string> errors = new List<string>();

            ValidateOpenScenes(errors);

            Report(errors, "Validate Open Scenes");
        }

        private static void ValidateContentAssets(List<string> errors)
        {
            ContentValidationSettings settings = LoadSettings();

            if (settings == null)
            {
                errors.Add(
                    "ContentValidationSettings asset not found. Create one via Create > Settings > Content Validation Settings."
                );

                return;
            }

            ValidateConfigs(settings, errors);
            ValidatePrefabs(settings, errors);
        }

        private static void ValidateConfigs(ContentValidationSettings settings, List<string> errors)
        {
            foreach (string folder in settings.ConfigFolders)
            {
                if (string.IsNullOrWhiteSpace(folder))
                {
                    continue;
                }

                string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { folder });

                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    ScriptableObject asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);

                    if (asset is IValidatable validatable)
                    {
                        TryValidate(validatable, path, errors);
                    }
                }
            }
        }

        private static void ValidatePrefabs(ContentValidationSettings settings, List<string> errors)
        {
            foreach (string folder in settings.PrefabFolders)
            {
                if (string.IsNullOrWhiteSpace(folder))
                {
                    continue;
                }

                string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { folder });

                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    GameObject root = PrefabUtility.LoadPrefabContents(path);

                    try
                    {
                        IValidatable[] validatables = root.GetComponentsInChildren<IValidatable>(true);

                        foreach (IValidatable validatable in validatables)
                        {
                            string context = $"{path} ({validatable.GetType().Name})";
                            TryValidate(validatable, context, errors);
                        }
                    }
                    finally
                    {
                        PrefabUtility.UnloadPrefabContents(root);
                    }
                }
            }
        }

        private static void ValidateOpenScenes(List<string> errors)
        {
            for (int sceneIndex = 0; sceneIndex < EditorSceneManager.sceneCount; sceneIndex++)
            {
                Scene scene = EditorSceneManager.GetSceneAt(sceneIndex);

                if (!scene.isLoaded)
                {
                    continue;
                }

                GameObject[] roots = scene.GetRootGameObjects();

                foreach (GameObject root in roots)
                {
                    IValidatable[] validatables = root.GetComponentsInChildren<IValidatable>(true);

                    foreach (IValidatable validatable in validatables)
                    {
                        string objectName = validatable is Component component
                            ? component.gameObject.name
                            : validatable.GetType().Name;

                        string context = $"{scene.path} / {objectName} ({validatable.GetType().Name})";
                        TryValidate(validatable, context, errors);
                    }
                }
            }
        }

        private static void TryValidate(
            IValidatable validatable,
            string context,
            List<string> errors)
        {
            try
            {
                validatable.Validate();
            }
            catch (Exception exception)
            {
                errors.Add($"{context}: {exception.Message}");
            }
        }

        private static ContentValidationSettings LoadSettings()
        {
            string[] guids = AssetDatabase.FindAssets("t:ContentValidationSettings");

            if (guids.Length == 0)
            {
                return null;
            }

            if (guids.Length > 1)
            {
                StringBuilder paths = new StringBuilder();

                foreach (string guid in guids)
                {
                    paths.AppendLine($"- {AssetDatabase.GUIDToAssetPath(guid)}");
                }

                Debug.LogError(
                    $"[Content Validation] Found {guids.Length} ContentValidationSettings assets; using the first. Keep only one:\n{paths}"
                );
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<ContentValidationSettings>(path);
        }

        private static void Report(List<string> errors, string title)
        {
            if (errors.Count == 0)
            {
                Debug.Log($"[Content Validation] {title}: OK");
                return;
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"[Content Validation] {title}: {errors.Count} error(s)");

            foreach (string error in errors)
            {
                builder.AppendLine($"- {error}");
            }

            Debug.LogError(builder.ToString());
        }
    }
}
