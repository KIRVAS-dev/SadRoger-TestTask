using System.Collections.Generic;
using UnityEngine;

namespace ContentValidation
{
    [CreateAssetMenu(menuName = "Settings/Content Validation Settings", fileName = "ContentValidationSettings")]
    public sealed class ContentValidationSettings : ScriptableObject
    {
        [Tooltip("AssetDatabase folder paths, e.g. Assets/_Project/Configs")]
        [SerializeField] private List<string> _configFolders = new List<string>();

        [Tooltip("AssetDatabase folder paths, e.g. Assets/_Project/Prefabs")]
        [SerializeField] private List<string> _prefabFolders = new List<string>();

        public IReadOnlyList<string> ConfigFolders => _configFolders;

        public IReadOnlyList<string> PrefabFolders => _prefabFolders;
    }
}
