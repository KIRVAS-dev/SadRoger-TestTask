#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace ProjectDebug
{
    [InitializeOnLoad]
    internal static class TemplateIdentityWarning
    {
        private const string TemplateProjectName = "Template-UnityProject";
        private const string TemplateBundleIdMarker = "unity.template";

        static TemplateIdentityWarning()
        {
            string projectFolderName = Path.GetFileName(Path.GetDirectoryName(Application.dataPath));

            if (projectFolderName == TemplateProjectName)
            {
                return;
            }

            if (PlayerSettings.productName == TemplateProjectName)
            {
                Debug.LogWarning(
                    $"Player Settings: Product Name is still '{TemplateProjectName}' — set it for this project (skill new-project)"
                );
            }

            string bundleId = PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Standalone);

            if (bundleId.Contains(TemplateBundleIdMarker, StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogWarning(
                    $"Player Settings: Bundle Identifier '{bundleId}' comes from the URP template — set it for this project (skill new-project)"
                );
            }
        }
    }
}
#endif
