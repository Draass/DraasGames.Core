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

        public DLogLevel MinimumLevel => _minimumLevel;

#if UNITY_EDITOR
        private void OnValidate()
        {
            DLogger.MinimumLevel = _minimumLevel;
        }
#endif
    }
}
