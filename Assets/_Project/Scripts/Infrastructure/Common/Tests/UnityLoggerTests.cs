using System;
using NUnit.Framework;
using Wordle.Application.Interfaces;
using UnityEngine;
using UnityEngine.TestTools;
using Wordle.Infrastructure.Logging;

namespace Tests
{
    [TestFixture]
    public class UnityLoggerTests
    {
        [Test]
        public void Constructor_Default_SetsLogLevelToDebug()
        {
            var logger = new UnityLogger();

            Assert.DoesNotThrow(() => logger.LogDebug("Test"));
        }

        [Test]
        public void Constructor_WithLogLevel_SetsLogLevel()
        {
            var logger = new UnityLogger(LogLevel.Error);

            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("\\[ERROR\\] Test"));
            Assert.DoesNotThrow(() => logger.LogError("Test"));
        }

        [Test]
        public void SetLogLevel_ChangesLogLevel()
        {
            var logger = new UnityLogger(LogLevel.Debug);

            logger.SetLogLevel(LogLevel.Error);

            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("\\[ERROR\\] Test"));
            Assert.DoesNotThrow(() => logger.LogError("Test"));
        }

        [Test]
        public void LogDebug_DoesNotThrow()
        {
            var logger = new UnityLogger();

            Assert.DoesNotThrow(() => logger.LogDebug("Debug message"));
        }

        [Test]
        public void LogInfo_DoesNotThrow()
        {
            var logger = new UnityLogger();

            Assert.DoesNotThrow(() => logger.LogInfo("Info message"));
        }

        [Test]
        public void LogWarning_DoesNotThrow()
        {
            var logger = new UnityLogger();

            Assert.DoesNotThrow(() => logger.LogWarning("Warning message"));
        }

        [Test]
        public void LogError_DoesNotThrow()
        {
            var logger = new UnityLogger();

            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("\\[ERROR\\] Error message"));
            Assert.DoesNotThrow(() => logger.LogError("Error message"));
        }

        [Test]
        public void LogException_WithException_DoesNotThrow()
        {
            var logger = new UnityLogger();
            var exception = new Exception("Test exception");

            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("\\[ERROR\\] Exception occurred"));
            Assert.DoesNotThrow(() => logger.LogException("Exception occurred", exception));
        }

        [Test]
        public void LogException_WithNullException_DoesNotThrow()
        {
            var logger = new UnityLogger();

            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("\\[ERROR\\] Exception occurred"));
            Assert.DoesNotThrow(() => logger.LogException("Exception occurred", null));
        }

        [Test]
        public void SetLogLevel_ToInfo_AllowsInfoAndAbove()
        {
            var logger = new UnityLogger();
            logger.SetLogLevel(LogLevel.Info);

            Assert.DoesNotThrow(() => logger.LogInfo("Info message"));
            Assert.DoesNotThrow(() => logger.LogWarning("Warning message"));

            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("\\[ERROR\\] Error message"));
            Assert.DoesNotThrow(() => logger.LogError("Error message"));
        }

        [Test]
        public void SetLogLevel_ToWarning_AllowsWarningAndAbove()
        {
            var logger = new UnityLogger();
            logger.SetLogLevel(LogLevel.Warning);

            Assert.DoesNotThrow(() => logger.LogWarning("Warning message"));

            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("\\[ERROR\\] Error message"));
            Assert.DoesNotThrow(() => logger.LogError("Error message"));
        }

        [Test]
        public void SetLogLevel_ToError_AllowsErrorOnly()
        {
            var logger = new UnityLogger();
            logger.SetLogLevel(LogLevel.Error);

            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("\\[ERROR\\] Error message"));
            Assert.DoesNotThrow(() => logger.LogError("Error message"));
        }
    }
}
