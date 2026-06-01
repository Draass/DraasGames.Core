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

        /// <summary>
        /// Raised for every message that passes the <see cref="MinimumLevel"/> gate, just before it
        /// is forwarded to the registered <see cref="ILoggerService"/> sinks. Carries a structured
        /// <see cref="DLogEntry"/> (level, message, sender, future tags) for console-style views.
        /// </summary>
        public static event Action<DLogEntry> MessageLogged;

        /// <summary>
        /// True while DLogger is forwarding a message to its sinks. A listener on
        /// <see cref="UnityEngine.Application.logMessageReceived"/> can check this to skip the echo
        /// Unity raises for messages that originated from DLogger, avoiding duplicate records.
        /// </summary>
        public static bool IsDispatching { get; private set; }

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

            Dispatch(DLogLevel.Info, message, sender, null);
        }

        public static void LogWarning(string message, object sender = null)
        {
            if (!ShouldLog(DLogLevel.Warning))
            {
                return;
            }

            Dispatch(DLogLevel.Warning, message, sender, null);
        }

        public static void LogError(string message, object sender = null)
        {
            if (!ShouldLog(DLogLevel.Error))
            {
                return;
            }

            Dispatch(DLogLevel.Error, message, sender, null);
        }

        public static void LogException(Exception exception)
        {
            if (!ShouldLog(DLogLevel.Exception))
            {
                return;
            }

            Dispatch(DLogLevel.Exception, exception?.Message, null, exception);
        }

        private static void Dispatch(DLogLevel level, string message, object sender, Exception exception)
        {
            // Raise the structured signal first so listeners (e.g. the editor console window) record a
            // clean entry. Only allocate the entry when something is actually listening.
            if (MessageLogged != null)
            {
                var entry = new DLogEntry(level, message, sender?.GetType().Name, exception, DLogSource.DLogger);
                MessageLogged.Invoke(entry);
            }

            // Forwarding to sinks calls UnityEngine.Debug, which makes Unity re-raise the same message
            // through Application.logMessageReceived. IsDispatching lets that listener drop the echo.
            IsDispatching = true;
            try
            {
                foreach (var logger in Loggers)
                {
                    switch (level)
                    {
                        case DLogLevel.Info:
                            logger.Log(message, sender);
                            break;
                        case DLogLevel.Warning:
                            logger.LogWarning(message, sender);
                            break;
                        case DLogLevel.Error:
                            logger.LogError(message, sender);
                            break;
                        case DLogLevel.Exception:
                            logger.LogException(exception);
                            break;
                    }
                }
            }
            finally
            {
                IsDispatching = false;
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
