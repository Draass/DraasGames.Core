#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using DraasGames.Core.Runtime.Infrastructure.Logger;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditorInternal;
using UnityEngine;

namespace Packages.DraasGames.Core.Editor.Console
{
    /// <summary>
    /// Editor-side wrapper around a <see cref="DLogEntry"/> with everything the console view needs:
    /// a captured stack trace, a resolved call site for navigation, a timestamp and a collapse count.
    /// </summary>
    internal sealed class DConsoleEntry
    {
        public DLogLevel Level { get; }
        public string Message { get; }
        public string Sender { get; }
        public string StackTrace { get; }
        public DLogSource Source { get; }
        public IReadOnlyList<string> Tags { get; }
        public LogType LogType { get; }

        /// <summary>Absolute path of the originating source file, or <c>null</c> if unknown.</summary>
        public string FilePath { get; }
        public int Line { get; }
        public DateTime Time { get; }

        /// <summary>Number of identical consecutive messages (reserved for a future Collapse mode).</summary>
        public int Count { get; set; }

        /// <summary>True for compiler errors; these survive a manual Clear so they stay visible.</summary>
        public bool IsCompileError { get; }

        public DConsoleEntry(
            DLogLevel level,
            string message,
            string sender,
            string stackTrace,
            DLogSource source,
            IReadOnlyList<string> tags,
            LogType logType,
            string filePath,
            int line,
            DateTime time,
            bool isCompileError = false)
        {
            Level = level;
            Message = message ?? string.Empty;
            Sender = sender;
            StackTrace = stackTrace ?? string.Empty;
            Source = source;
            Tags = tags ?? Array.Empty<string>();
            LogType = logType;
            FilePath = filePath;
            Line = line;
            Time = time;
            Count = 1;
            IsCompileError = isCompileError;
        }
    }

    /// <summary>
    /// Always-on recorder that feeds <see cref="DConsoleWindow"/>. Captures two streams into one
    /// buffer: structured <see cref="DLogger.MessageLogged"/> entries (which respect
    /// <see cref="DLogger.MinimumLevel"/>) and raw Unity logs via
    /// <see cref="Application.logMessageReceived"/>. DLogger's own echo through Unity is dropped while
    /// <see cref="DLogger.IsDispatching"/> is set, so messages are never recorded twice.
    /// </summary>
    /// <remarks>
    /// The buffer lives in a static field and therefore resets on domain reload / script recompile,
    /// matching Unity's own console default. Capture assumes main-thread logging; background-thread
    /// logging is a known follow-up.
    /// </remarks>
    [InitializeOnLoad]
    internal static class DConsoleRecorder
    {
        private const int LevelCount = 4; // Info, Warning, Error, Exception
        private const string ClearOnPlayKey = "DraasGames.DConsole.ClearOnPlay";

        private static readonly List<DConsoleEntry> Entries = new(256);
        private static readonly int[] Counts = new int[LevelCount];
        private static readonly string ProjectRoot =
            Directory.GetParent(Application.dataPath)?.FullName?.Replace('\\', '/');

        private static readonly Regex UnityTraceRegex =
            new(@"\(at (.+?):(\d+)\)", RegexOptions.Compiled);

        /// <summary>Raised whenever the buffer changes. The window coalesces this into a throttled refresh.</summary>
        public static event Action Changed;

        /// <summary>Maximum number of retained entries; the oldest are trimmed past this.</summary>
        public static int Capacity { get; set; } = 5000;

        public static IReadOnlyList<DConsoleEntry> Snapshot => Entries;

        public static bool ClearOnPlay
        {
            get => EditorPrefs.GetBool(ClearOnPlayKey, true);
            set => EditorPrefs.SetBool(ClearOnPlayKey, value);
        }

        static DConsoleRecorder()
        {
            DLogger.MessageLogged += OnDLoggerMessage;
            Application.logMessageReceived += OnUnityMessage;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            CompilationPipeline.compilationStarted += OnCompilationStarted;
            CompilationPipeline.assemblyCompilationFinished += OnAssemblyCompilationFinished;
        }

        public static int GetCount(DLogLevel level)
        {
            var index = (int)level;
            return index >= 0 && index < LevelCount ? Counts[index] : 0;
        }

        public static void Clear()
        {
            // Keep compiler errors so clearing the console does not hide what is blocking the build.
            Entries.RemoveAll(entry => !entry.IsCompileError);
            RecountLevels();
            Changed?.Invoke();
        }

        /// <summary>Opens the entry's source file at its line in the configured external editor.</summary>
        public static bool TryOpenInEditor(DConsoleEntry entry)
        {
            if (entry == null || string.IsNullOrEmpty(entry.FilePath))
            {
                return false;
            }

            InternalEditorUtility.OpenFileAtLineExternal(entry.FilePath, Mathf.Max(1, entry.Line));
            return true;
        }

        /// <summary>Opens an arbitrary source path (project-relative or absolute) at the given line.</summary>
        public static void OpenFile(string path, int line)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            InternalEditorUtility.OpenFileAtLineExternal(ToAbsolute(path), Mathf.Max(1, line));
        }

        private static void OnDLoggerMessage(DLogEntry entry)
        {
            BuildDLoggerTrace(out var display, out var file, out var line);
            Add(new DConsoleEntry(
                entry.Level,
                entry.Message,
                entry.Sender,
                display,
                entry.Source,
                entry.Tags,
                ToLogType(entry.Level),
                file,
                line,
                DateTime.Now));
        }

        private static void OnUnityMessage(string condition, string stackTrace, LogType type)
        {
            // While DLogger forwards to its sinks, Unity re-raises the same message here. We already
            // recorded the structured version via OnDLoggerMessage, so drop this echo.
            if (DLogger.IsDispatching)
            {
                return;
            }

            ParseUnityTrace(stackTrace, out var file, out var line);
            Add(new DConsoleEntry(
                ToLevel(type),
                condition,
                null,
                stackTrace,
                DLogSource.Unity,
                Array.Empty<string>(),
                type,
                file,
                line,
                DateTime.Now));
        }

        private static void Add(DConsoleEntry entry)
        {
            Entries.Add(entry);

            var levelIndex = (int)entry.Level;
            if (levelIndex >= 0 && levelIndex < LevelCount)
            {
                Counts[levelIndex]++;
            }

            var overflow = Entries.Count - Capacity;
            if (overflow > 0)
            {
                for (var i = 0; i < overflow; i++)
                {
                    var removedLevel = (int)Entries[i].Level;
                    if (removedLevel >= 0 && removedLevel < LevelCount)
                    {
                        Counts[removedLevel]--;
                    }
                }

                Entries.RemoveRange(0, overflow);
            }

            Changed?.Invoke();
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode && ClearOnPlay)
            {
                Clear();
            }
        }

        private static void OnCompilationStarted(object context)
        {
            // A fresh compile round invalidates the previously reported compiler errors.
            RemoveCompileErrors();
        }

        private static void OnAssemblyCompilationFinished(string assemblyPath, CompilerMessage[] messages)
        {
            if (messages == null)
            {
                return;
            }

            foreach (var message in messages)
            {
                if (message.type != CompilerMessageType.Error)
                {
                    continue;
                }

                Add(new DConsoleEntry(
                    DLogLevel.Error,
                    message.message,
                    null,
                    string.Empty,
                    DLogSource.Unity,
                    Array.Empty<string>(),
                    LogType.Error,
                    ToAbsolute(message.file),
                    message.line,
                    DateTime.Now,
                    isCompileError: true));
            }
        }

        private static void RemoveCompileErrors()
        {
            if (Entries.RemoveAll(entry => entry.IsCompileError) > 0)
            {
                RecountLevels();
                Changed?.Invoke();
            }
        }

        private static void RecountLevels()
        {
            Array.Clear(Counts, 0, Counts.Length);
            for (var i = 0; i < Entries.Count; i++)
            {
                var level = (int)Entries[i].Level;
                if (level >= 0 && level < LevelCount)
                {
                    Counts[level]++;
                }
            }
        }

        private static LogType ToLogType(DLogLevel level)
        {
            switch (level)
            {
                case DLogLevel.Warning:
                    return LogType.Warning;
                case DLogLevel.Error:
                    return LogType.Error;
                case DLogLevel.Exception:
                    return LogType.Exception;
                default:
                    return LogType.Log;
            }
        }

        private static DLogLevel ToLevel(LogType type)
        {
            switch (type)
            {
                case LogType.Warning:
                    return DLogLevel.Warning;
                case LogType.Error:
                case LogType.Assert:
                    return DLogLevel.Error;
                case LogType.Exception:
                    return DLogLevel.Exception;
                default:
                    return DLogLevel.Info;
            }
        }

        private static void BuildDLoggerTrace(out string display, out string navFile, out int navLine)
        {
            navFile = null;
            navLine = 0;

            StackFrame[] frames;
            try
            {
                frames = new StackTrace(true).GetFrames();
            }
            catch
            {
                frames = null;
            }

            if (frames == null)
            {
                display = string.Empty;
                return;
            }

            var builder = new StringBuilder();
            foreach (var frame in frames)
            {
                var method = frame.GetMethod();
                var type = method?.DeclaringType;
                if (type == null)
                {
                    continue;
                }

                // Skip the logging plumbing so the first frame is the real call site.
                if (type == typeof(DLogger) || type == typeof(DConsoleRecorder))
                {
                    continue;
                }

                var fileName = frame.GetFileName();
                var hasFile = !string.IsNullOrEmpty(fileName);
                var lineNumber = frame.GetFileLineNumber();

                builder.Append(type.FullName).Append('.').Append(method.Name).Append(" ()");
                if (hasFile)
                {
                    builder.Append(" (at ").Append(ToProjectRelative(fileName)).Append(':')
                        .Append(lineNumber).Append(')');
                }

                builder.Append('\n');

                if (navFile == null && hasFile)
                {
                    navFile = fileName.Replace('\\', '/');
                    navLine = lineNumber;
                }
            }

            display = builder.ToString();
        }

        private static void ParseUnityTrace(string stackTrace, out string navFile, out int navLine)
        {
            navFile = null;
            navLine = 0;

            if (string.IsNullOrEmpty(stackTrace))
            {
                return;
            }

            var match = UnityTraceRegex.Match(stackTrace);
            if (!match.Success)
            {
                return;
            }

            if (!int.TryParse(match.Groups[2].Value, out navLine))
            {
                navLine = 0;
            }

            navFile = ToAbsolute(match.Groups[1].Value);
        }

        private static string ToProjectRelative(string absolutePath)
        {
            var normalized = absolutePath.Replace('\\', '/');

            var index = normalized.IndexOf("/Assets/", StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                return normalized.Substring(index + 1);
            }

            index = normalized.IndexOf("/Packages/", StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                return normalized.Substring(index + 1);
            }

            return normalized;
        }

        private static string ToAbsolute(string path)
        {
            var normalized = path.Replace('\\', '/');

            if (Path.IsPathRooted(normalized) || string.IsNullOrEmpty(ProjectRoot))
            {
                return normalized;
            }

            return ProjectRoot + "/" + normalized;
        }
    }
}
#endif
