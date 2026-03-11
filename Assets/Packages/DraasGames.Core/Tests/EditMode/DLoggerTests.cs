using System;
using DraasGames.Core.Runtime.Infrastructure.Logger;
using NUnit.Framework;
using UnityEngine;

namespace _Project.Scripts.DraasGames.Tests.EditMode
{
    [TestFixture]
    public class DLoggerTests
    {
        private TestLoggerService _logger;

        [SetUp]
        public void SetUp()
        {
            _logger = new TestLoggerService();
            DLogger.RemoveAllLoggers();
            DLogger.AddLogger(_logger);
        }

        [TearDown]
        public void TearDown()
        {
            DLogger.RemoveAllLoggers();
            DLogger.ReloadSettings();
        }

        [Test]
        public void ReloadSettings_ShouldApplyValueFromProjectSettingsAsset()
        {
            var settings = Resources.Load<DLoggerSettings>(DLoggerSettings.ResourcePath);

            Assert.That(settings, Is.Not.Null);

            DLogger.MinimumLevel = DLogLevel.None;
            DLogger.ReloadSettings();

            Assert.That(DLogger.MinimumLevel, Is.EqualTo(settings.MinimumLevel));
        }

        [Test]
        public void Log_ShouldSkipMessagesBelowConfiguredMinimumLevel()
        {
            DLogger.MinimumLevel = DLogLevel.Warning;

            DLogger.Log("info");
            DLogger.LogWarning("warning");
            DLogger.LogError("error");

            Assert.That(_logger.InfoCount, Is.EqualTo(0));
            Assert.That(_logger.WarningCount, Is.EqualTo(1));
            Assert.That(_logger.ErrorCount, Is.EqualTo(1));
        }

        [Test]
        public void LogException_ShouldRespectConfiguredMinimumLevel()
        {
            var exception = new InvalidOperationException("boom");

            DLogger.MinimumLevel = DLogLevel.None;
            DLogger.LogException(exception);
            Assert.That(_logger.ExceptionCount, Is.EqualTo(0));

            DLogger.MinimumLevel = DLogLevel.Exception;
            DLogger.LogException(exception);

            Assert.That(_logger.ExceptionCount, Is.EqualTo(1));
        }

        private sealed class TestLoggerService : ILoggerService
        {
            public int InfoCount { get; private set; }
            public int WarningCount { get; private set; }
            public int ErrorCount { get; private set; }
            public int ExceptionCount { get; private set; }

            public void Log(string message, object sender = null)
            {
                InfoCount++;
            }

            public void LogWarning(string message, object sender = null)
            {
                WarningCount++;
            }

            public void LogError(string message, object sender = null)
            {
                ErrorCount++;
            }

            public void LogException(Exception exception)
            {
                ExceptionCount++;
            }
        }
    }
}
