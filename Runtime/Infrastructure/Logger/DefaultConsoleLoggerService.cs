using System;
using UnityEngine;

namespace DraasGames.Core.Runtime.Infrastructure.Logger
{
    public class DefaultConsoleLoggerService : ILoggerService
    {
        public void Log(string message, object sender = null)
        {
            Debug.Log(FormatMessage(message, sender));
        }

        public void LogWarning(string message, object sender = null)
        {
            Debug.LogWarning(FormatMessage(message, sender));
        }

        public void LogError(string message, object sender = null)
        {
            Debug.LogError(FormatMessage(message, sender));
        }

        public void LogException(Exception exception)
        {
            Debug.LogException(exception);
        }

        private string FormatMessage(string message, object sender = null)
        {
            if (sender == null)
            {
                return message;
            }
            else
            {
                return $"[{sender.GetType().Name}]: {message}";
            }
        }
    }
}