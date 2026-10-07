using UnityEditor;

namespace ContentValidation.Editor
{
    public static class ContentValidationMenu
    {
        private const string Root = "Tools/ContentValidation/";

        [MenuItem(Root + "Validate All")]
        private static void ValidateAll()
        {
            ContentValidationRunner.ValidateAll();
        }

        [MenuItem(Root + "Validate Content Assets")]
        private static void ValidateContentAssets()
        {
            ContentValidationRunner.ValidateContentAssets();
        }

        [MenuItem(Root + "Validate Open Scenes")]
        private static void ValidateOpenScenes()
        {
            ContentValidationRunner.ValidateOpenScenes();
        }
    }
}
