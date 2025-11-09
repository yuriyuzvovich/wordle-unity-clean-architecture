using System.Collections.Generic;
using System.Linq;
using Wordle.Application.Interfaces;

namespace Wordle.Infrastructure.RemoteServices
{
    public class PlaceholderAnalyticsService : IAnalyticsService
    {
        private readonly ILogService _logService;

        public PlaceholderAnalyticsService(ILogService logService)
        {
            _logService = logService ?? throw new System.ArgumentNullException(nameof(logService));
        }

        public void TrackEvent(string eventName)
        {
            _logService.LogInfo($"[Analytics] Event tracked: {eventName}");
        }

        public void TrackEvent(string eventName, Dictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                _logService.LogInfo($"[Analytics] Event tracked: {eventName}");
                return;
            }

            var paramsString = string.Join(", ", parameters.Select(kv => $"{kv.Key}:{kv.Value}"));
            _logService.LogInfo($"[Analytics] Event tracked: {eventName} with parameters: {paramsString}");
        }

        public void SetUserProperty(string propertyName, string value)
        {
            _logService.LogInfo($"[Analytics] User property set: {propertyName} = {value}");
        }
    }
}
