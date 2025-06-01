using System;
using UnityEngine;

namespace DraasGames.Core.Runtime.Infrastructure.Logger
{
    public class FormattedConsoleLoggerService : ILoggerService
    {
        private const string ColorContext = "#66CCFF";
        private const string ColorRegular = "#FFFFFF";
        private const string ColorWarning = "#FF9900";
        private const string ColorError = "#FF0000";
        
        public void Log(string message, object sender = null)
        {
            Debug.Log(FormatMessage(message, sender, ColorRegular));
        }

        public void LogWarning(string message, object sender = null)
        {
            Debug.LogWarning(FormatMessage(message, sender, ColorWarning));
        }
        
        public void LogError(string message, object sender = null)
        {
            Debug.LogError(FormatMessage(message, sender, ColorError));
        }

        public void LogException(Exception exception)
        {
            Debug.LogException(exception);
        }
        
        private string FormatMessage(string message, object sender, string messageColor)
        {
            if (sender != null)
            {
                string senderName = sender.GetType().Name;
                return $"<color={ColorContext}>[{senderName}]</color>: <color={messageColor}>{message}</color>";
            }
            else
            {
                return $"<color={messageColor}>{message}</color>";
            }
        }
    }
}