using System;

namespace Wordle.Application.Events
{
    /// <summary>
    /// Application event: The user interface language has been changed.
    /// Used to notify UI components to refresh their text.
    /// </summary>
    public struct LanguageChangedEvent
    {
        public readonly string EventId;
        public readonly DateTime Timestamp;
        public readonly string LanguageCode;

        public LanguageChangedEvent(string languageCode)
        {
            EventId = nameof(LanguageChangedEvent);
            Timestamp = DateTime.UtcNow;
            LanguageCode = languageCode;
        }

        public override string ToString()
        {
            return $"[{EventId}] Language changed to '{LanguageCode}'";
        }
    }
}
