using Wordle.Core.Theme;

namespace Wordle.Application.DTOs
{
    public readonly struct ThemeDto
    {
        public readonly ThemeType ThemeType;
        public readonly ColorScheme ColorScheme;

        public ThemeDto(ThemeType themeType, ColorScheme colorScheme)
        {
            ThemeType = themeType;
            ColorScheme = colorScheme;
        }
    }
}
