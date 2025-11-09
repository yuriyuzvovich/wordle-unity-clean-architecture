using System;
using System.Collections.Generic;
using Wordle.Application.Interfaces;

namespace Wordle.Application.Tests.Mocks
{
    public class MockLogService : ILogService
    {
        public List<string> InfoLogs = new List<string>();
        public List<string> WarningLogs = new List<string>();
        public List<string> ErrorLogs = new List<string>();

        public void LogDebug(string message)
        {
        }

        public void LogInfo(string message)
        {
            InfoLogs.Add(message);
        }

        public void LogWarning(string message)
        {
            WarningLogs.Add(message);
        }

        public void LogError(string message)
        {
            ErrorLogs.Add(message);
        }

        public void LogException(string message, Exception exception)
        {
            ErrorLogs.Add($"{message} - Exception: {exception.Message}");
        }

        public void SetLogLevel(LogLevel level)
        {
        }

        public void Clear()
        {
            InfoLogs.Clear();
            WarningLogs.Clear();
            ErrorLogs.Clear();
        }
    }
}
