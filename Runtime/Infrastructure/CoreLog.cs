namespace DraasGames.Core.Runtime.Infrastructure
{
    /// <summary>
    /// Internal logging facade for the Core package. Routes through DLogger when the optional
    /// com.draasgames.dlogger package is installed (DRAASGAMES_DLOGGER), otherwise falls back to
    /// UnityEngine.Debug with the same [SenderType] prefix convention.
    /// </summary>
    internal static class CoreLog
    {
        public static void Error(string message, object sender = null)
        {
#if DRAASGAMES_DLOGGER
            DraasGames.Logging.DLogger.LogError(message, sender);
#else
            UnityEngine.Debug.LogError(Format(message, sender));
#endif
        }

        public static void Warning(string message, object sender = null)
        {
#if DRAASGAMES_DLOGGER
            DraasGames.Logging.DLogger.LogWarning(message, sender);
#else
            UnityEngine.Debug.LogWarning(Format(message, sender));
#endif
        }

        public static void Info(string message, object sender = null)
        {
#if DRAASGAMES_DLOGGER
            DraasGames.Logging.DLogger.Log(message, sender);
#else
            UnityEngine.Debug.Log(Format(message, sender));
#endif
        }

#if !DRAASGAMES_DLOGGER
        private static string Format(string message, object sender)
        {
            return sender != null ? $"[{sender.GetType().Name}]: {message}" : message;
        }
#endif
    }
}
