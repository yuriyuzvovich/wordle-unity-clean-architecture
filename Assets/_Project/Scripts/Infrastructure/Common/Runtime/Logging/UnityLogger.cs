using System;
using Wordle.Application.Interfaces;
using Debug = UnityEngine.Debug;

namespace Wordle.Infrastructure.Logging
{
    public class UnityLogger : ILogService
    {
        private LogLevel _currentLogLevel;

        public UnityLogger() : this(LogLevel.Debug)
        {
        }

        public UnityLogger(LogLevel logLevel)
        {
            _currentLogLevel = logLevel;
        }

        public void LogDebug(string message)
        {
            if (_currentLogLevel <= LogLevel.Debug)
            {
                Debug.Log($"[DEBUG] {message}");
            }
        }
        
        public void LogInfo(string message)
        {
            if (_currentLogLevel <= LogLevel.Info)
            {
                Debug.Log($"[INFO] {message}");
            }
        }

        public void LogWarning(string message)
        {
            if (_currentLogLevel <= LogLevel.Warning)
            {
                Debug.LogWarning($"[WARNING] {message}");
            }
        }

        public void LogError(string message)
        {
            if (_currentLogLevel <= LogLevel.Error)
            {
                Debug.LogError($"[ERROR] {message}");
            }
        }

        public void LogException(string message, Exception exception)
        {
            if (_currentLogLevel <= LogLevel.Error)
            {
                if (exception != null)
                {
                    Debug.LogError($"[ERROR] {message}\nException: {exception.GetType().Name}\nMessage: {exception.Message}\nStackTrace: {exception.StackTrace}");
                }
                else
                {
                    Debug.LogError($"[ERROR] {message}");
                }
            }
        }

        public void SetLogLevel(LogLevel level)
        {
            _currentLogLevel = level;
        }
    }
}
