using System.Collections.Generic;

namespace Wordle.Application.Interfaces
{
    public interface IAnalyticsService
    {
        void TrackEvent(string eventName);
        void TrackEvent(string eventName, Dictionary<string, object> parameters);
        void SetUserProperty(string propertyName, string value);
    }
}
