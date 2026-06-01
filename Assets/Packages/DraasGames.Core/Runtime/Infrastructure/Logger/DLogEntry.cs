using System;
using System.Collections.Generic;

namespace DraasGames.Core.Runtime.Infrastructure.Logger
{
    /// <summary>
    /// Where a <see cref="DLogEntry"/> originated.
    /// </summary>
    public enum DLogSource
    {
        /// <summary>Logged through <see cref="DLogger"/> — level, sender and tags are known.</summary>
        DLogger = 0,

        /// <summary>Captured from Unity's log stream (Application.logMessageReceived).</summary>
        Unity = 1
    }

    /// <summary>
    /// A single structured log record carried by <see cref="DLogger.MessageLogged"/>.
    /// Holds everything a console-style view needs to render and filter a message, including a
    /// <see cref="Tags"/> slot that stays empty until DLogger gains tag support — so tag filtering
    /// can be added later without touching the capture pipeline or <see cref="ILoggerService"/>.
    /// </summary>
    public readonly struct DLogEntry
    {
        public DLogLevel Level { get; }

        public string Message { get; }

        /// <summary>Type name of the sender object, or <c>null</c> when none was provided.</summary>
        public string Sender { get; }

        /// <summary>The exception for <see cref="DLogger.LogException"/> entries; otherwise <c>null</c>.</summary>
        public Exception Exception { get; }

        public DLogSource Source { get; }

        /// <summary>Tags attached to the message. Never <c>null</c>; empty until DLogger supports tags.</summary>
        public IReadOnlyList<string> Tags { get; }

        public DLogEntry(
            DLogLevel level,
            string message,
            string sender = null,
            Exception exception = null,
            DLogSource source = DLogSource.DLogger,
            IReadOnlyList<string> tags = null)
        {
            Level = level;
            Message = message;
            Sender = sender;
            Exception = exception;
            Source = source;
            Tags = tags ?? Array.Empty<string>();
        }
    }
}
