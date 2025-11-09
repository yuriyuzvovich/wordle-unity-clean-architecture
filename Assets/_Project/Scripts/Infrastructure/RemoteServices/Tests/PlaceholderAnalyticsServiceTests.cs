using System;
using System.Collections.Generic;
using NUnit.Framework;
using Wordle.Application.Interfaces;
using Wordle.Infrastructure.RemoteServices;

namespace Tests
{
    [TestFixture]
    public class PlaceholderAnalyticsServiceTests
    {
        private PlaceholderAnalyticsService _service;
        private MockLogService _logService;

        [SetUp]
        public void SetUp()
        {
            _logService = new MockLogService();
            _service = new PlaceholderAnalyticsService(_logService);
        }

        #region Constructor Tests

        [Test]
        public void Constructor_WithLogService_CreatesInstance()
        {
            var service = new PlaceholderAnalyticsService(_logService);

            Assert.IsNotNull(service);
        }

        [Test]
        public void Constructor_NullLogService_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new PlaceholderAnalyticsService(null));
        }

        #endregion

        #region TrackEvent Tests

        [Test]
        public void TrackEvent_WithEventName_LogsEvent()
        {
            _service.TrackEvent("TestEvent");

            Assert.AreEqual(1, _logService.InfoMessages.Count);
            Assert.IsTrue(_logService.InfoMessages[0].Contains("TestEvent"));
        }

        [Test]
        public void TrackEvent_WithParameters_LogsEventAndParameters()
        {
            var parameters = new Dictionary<string, object>
            {
                { "key1", "value1" },
                { "key2", 42 }
            };

            _service.TrackEvent("TestEvent", parameters);

            Assert.AreEqual(1, _logService.InfoMessages.Count);
            Assert.IsTrue(_logService.InfoMessages[0].Contains("TestEvent"));
        }

        [Test]
        public void TrackEvent_EmptyParameters_DoesNotThrow()
        {
            var parameters = new Dictionary<string, object>();

            Assert.DoesNotThrow(() => _service.TrackEvent("TestEvent", parameters));
        }

        [Test]
        public void TrackEvent_NullParameters_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _service.TrackEvent("TestEvent", null));
        }

        [Test]
        public void TrackEvent_MultipleEvents_LogsAllEvents()
        {
            _service.TrackEvent("Event1");
            _service.TrackEvent("Event2");
            _service.TrackEvent("Event3");

            Assert.AreEqual(3, _logService.InfoMessages.Count);
        }

        #endregion

        #region SetUserProperty Tests

        [Test]
        public void SetUserProperty_LogsUserProperty()
        {
            _service.SetUserProperty("UserLevel", "5");

            Assert.AreEqual(1, _logService.InfoMessages.Count);
            Assert.IsTrue(_logService.InfoMessages[0].Contains("UserLevel"));
            Assert.IsTrue(_logService.InfoMessages[0].Contains("5"));
        }

        [Test]
        public void SetUserProperty_NullValue_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _service.SetUserProperty("Property", null));
        }

        [Test]
        public void SetUserProperty_EmptyValue_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => _service.SetUserProperty("Property", ""));
        }

        [Test]
        public void SetUserProperty_MultipleProperties_LogsAllProperties()
        {
            _service.SetUserProperty("Prop1", "Value1");
            _service.SetUserProperty("Prop2", "Value2");

            Assert.AreEqual(2, _logService.InfoMessages.Count);
        }

        #endregion

        #region Test Helper Classes

        private class MockLogService : ILogService
        {
            public List<string> InfoMessages { get; } = new List<string>();
            public List<string> ErrorMessages { get; } = new List<string>();

            public void LogDebug(string message) { }

            public void LogInfo(string message)
            {
                InfoMessages.Add(message);
            }

            public void LogWarning(string message) { }

            public void LogError(string message)
            {
                ErrorMessages.Add(message);
            }

            public void LogException(string message, Exception exception)
            {
                ErrorMessages.Add(message);
            }

            public void SetLogLevel(LogLevel level) { }
        }

        #endregion
    }
}
