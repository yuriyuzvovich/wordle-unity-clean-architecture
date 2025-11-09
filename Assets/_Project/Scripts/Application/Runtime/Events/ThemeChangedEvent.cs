using System;
using Wordle.Core.Theme;

namespace Wordle.Application.Events
{
    public struct ThemeChangedEvent
    {
        public readonly string EventId;
        public readonly DateTime Timestamp;
        public readonly ThemeType ThemeType;
        public readonly ColorScheme ColorScheme;

        public ThemeChangedEvent(ThemeType themeType, ColorScheme colorScheme)
        {
            EventId = nameof(ThemeChangedEvent);
            Timestamp = DateTime.UtcNow;
            ThemeType = themeType;
            ColorScheme = colorScheme;
        }

        public override string ToString()
        {
            return $"[{EventId}] Theme changed to '{ThemeType}'";
        }
    }
}
