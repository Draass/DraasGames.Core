using System.Collections.Generic;
using UnityEngine;

namespace DraasGames.Core.Runtime.Infrastructure.Logger
{
    [CreateAssetMenu(
        fileName = "DLoggerSettings",
        menuName = "DraasGames/Logger/Settings")]
    public sealed class DLoggerSettings : ScriptableObject
    {
        public const string ResourcePath = "DraasGames/DLoggerSettings";
#if UNITY_EDITOR
        public const string DefaultAssetPath = "Assets/Resources/DraasGames/DLoggerSettings.asset";
#endif

        [SerializeField] private DLogLevel _minimumLevel = DLogLevel.Info;

        [Header("Tags")]
        [Tooltip("Tag names available to DLogger. Edit here, then click \"Generate Tags\" to (re)build the DLogTags constants class.")]
        [SerializeField] private List<string> _tags = new() { "Gameplay", "UI", "Network", "Audio" };

        [Tooltip("Namespace of the generated DLogTags class.")]
        [SerializeField] private string _generatedTagsNamespace = "DraasGames";

        [Tooltip("Project-relative path of the generated DLogTags.cs file.")]
        [SerializeField] private string _generatedTagsPath = "Assets/Packages/DraasGames/DLogger/DLogTags.cs";

        public DLogLevel MinimumLevel => _minimumLevel;

        public IReadOnlyList<string> Tags => _tags;

        public string GeneratedTagsNamespace => _generatedTagsNamespace;

        public string GeneratedTagsPath => _generatedTagsPath;

#if UNITY_EDITOR
        private void OnValidate()
        {
            DLogger.MinimumLevel = _minimumLevel;
        }
#endif
    }
}
