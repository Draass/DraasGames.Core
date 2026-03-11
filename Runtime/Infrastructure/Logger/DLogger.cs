using System;
using System.Collections.Generic;

namespace DraasGames.Core.Runtime.Infrastructure.Logger
{
    public static class DLogger
    {
        private static readonly HashSet<ILoggerService> Loggers = new();
        private static DLogLevel _minimumLevel = DLogLevel.Info;
        private static bool _minimumLevelInitialized;

        public static DLogLevel MinimumLevel
        {
            get
            {
                EnsureMinimumLevelInitialized();
                return _minimumLevel;
            }
            set
            {
                _minimumLevel = value;
                _minimumLevelInitialized = true;
            }
        }

        static DLogger()
        {
#if UNITY_EDITOR
            Loggers.Add(new FormattedConsoleLoggerService());
#elif UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE
            Loggers.Add(new DefaultConsoleLoggerService());
#endif
        }

        public static void AddLogger(ILoggerService logger)
        {
            Loggers.Add(logger);
        }

        public static void RemoveLogger(ILoggerService logger)
        {
            Loggers.Remove(logger);
        }

        public static void RemoveAllLoggers()
        {
            Loggers.Clear();
        }

        public static void ReloadSettings()
        {
            _minimumLevel = ResolveConfiguredMinimumLevel();
            _minimumLevelInitialized = true;
        }

        public static void Log(string message, object sender = null)
        {
            if (!ShouldLog(DLogLevel.Info))
            {
                return;
            }

            foreach (var logger in Loggers)
            {
                logger.Log(message, sender);
            }
        }

        public static void LogWarning(string message, object sender = null)
        {
            if (!ShouldLog(DLogLevel.Warning))
            {
                return;
            }

            foreach (var logger in Loggers)
            {
                logger.LogWarning(message, sender);
            }
        }

        public static void LogError(string message, object sender = null)
        {
            if (!ShouldLog(DLogLevel.Error))
            {
                return;
            }

            foreach (var logger in Loggers)
            {
                logger.LogError(message, sender);
            }
        }

        public static void LogException(Exception exception)
        {
            if (!ShouldLog(DLogLevel.Exception))
            {
                return;
            }

            foreach (var logger in Loggers)
            {
                logger.LogException(exception);
            }
        }

        private static bool ShouldLog(DLogLevel messageLevel)
        {
            if (MinimumLevel == DLogLevel.None)
            {
                return false;
            }

            return messageLevel >= MinimumLevel;
        }

        private static void EnsureMinimumLevelInitialized()
        {
            if (_minimumLevelInitialized)
            {
                return;
            }

            ReloadSettings();
        }

        private static DLogLevel ResolveConfiguredMinimumLevel()
        {
            var settings = UnityEngine.Resources.Load<DLoggerSettings>(DLoggerSettings.ResourcePath);
            return settings != null ? settings.MinimumLevel : DLogLevel.Info;
        }
    }
}
