using System;
using System.Collections.Generic;

namespace DraasGames.Core.Runtime.Infrastructure.Logger
{
    public class DLogger
    {
        private static readonly HashSet<ILoggerService> Loggers;

        public void AddLogger(ILoggerService logger)
        {
            Loggers.Add(logger);
        }
        
        public static void Log(string message, object sender = null)
        {
            foreach (var logger in Loggers)
            {
                logger.Log(message, sender);
            }
        }
        
        public static void LogWarning(string message, object sender = null)
        {
            foreach (var logger in Loggers)
            {
                logger.LogWarning(message, sender);
            }
        }
        
        public static void LogError(string message, object sender = null)
        {
            foreach (var logger in Loggers)
            {
                logger.LogError(message, sender);
            }
        }
        
        public static void LogException(Exception exception)
        {
            foreach (var logger in Loggers)
            {
                logger.LogException(exception);
            }
        }
    }
}