using System;

namespace Wordle.Application.Interfaces
{
    public interface ILogService
    {
        void LogDebug(string message);
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogException(string message, Exception exception);
        void SetLogLevel(LogLevel level);
    }
}
