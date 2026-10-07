using Infrastructure.ExtendedExceptions;

namespace ViewComponents.Level
{
    internal sealed class MissingLevelListConfigException : ExtendedException
    {
        internal MissingLevelListConfigException(string fieldName, string objectName)
            : base("level-1", $"Missing field {fieldName} on {objectName}") { }
    }

    internal sealed class MissingLevelPrefabException : ExtendedException
    {
        internal MissingLevelPrefabException(int index, string objectName)
            : base("level-2", $"Level prefab at index {index} is not assigned on {objectName}") { }
    }

    internal sealed class EmptyLevelListException : ExtendedException
    {
        internal EmptyLevelListException(string configName)
            : base("level-3", $"Level list {configName} is empty") { }
    }
}
