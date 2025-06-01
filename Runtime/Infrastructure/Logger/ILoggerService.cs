using System;

namespace DraasGames.Core.Runtime.Infrastructure.Logger
{
    public interface ILoggerService
    {
        public void Log(string message, object sender = null);
        
        public void LogWarning(string message, object sender = null);
        
        public void LogError(string message, object sender = null);
        
        public void LogException(Exception exception);
    }
}