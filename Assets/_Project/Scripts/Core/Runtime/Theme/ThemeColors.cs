namespace Wordle.Core.Theme
{
    public static class ThemeColors
    {
        public static ColorScheme GetColorScheme(ThemeType themeType)
        {
            return themeType switch
            {
                ThemeType.Light => LightTheme,
                ThemeType.Dark => DarkTheme,
                _ => DarkTheme
            };
        }

        public static ColorScheme DarkTheme => new ColorScheme(
            tileEmptyColor: new ColorRgba(0.2f, 0.2f, 0.2f, 1f),
            tileFilledColor: new ColorRgba(0.3f, 0.3f, 0.3f, 1f),
            tileAbsentColor: new ColorRgba(0.24f, 0.24f, 0.26f, 1f),
            tilePresentColor: new ColorRgba(0.71f, 0.65f, 0.26f, 1f),
            tileCorrectColor: new ColorRgba(0.42f, 0.68f, 0.39f, 1f),
            tileTextColor: new ColorRgba(1f, 1f, 1f, 1f),
            keyDefaultColor: new ColorRgba(0.51f, 0.51f, 0.53f, 1f),
            keyAbsentColor: new ColorRgba(0.24f, 0.24f, 0.26f, 1f),
            keyPresentColor: new ColorRgba(0.71f, 0.65f, 0.26f, 1f),
            keyCorrectColor: new ColorRgba(0.42f, 0.68f, 0.39f, 1f),
            keyTextColor: new ColorRgba(1f, 1f, 1f, 1f),
            backgroundColor: new ColorRgba(0.07f, 0.07f, 0.07f, 1f),
            panelColor: new ColorRgba(0.15f, 0.15f, 0.15f, 1f),
            textPrimaryColor: new ColorRgba(1f, 1f, 1f, 1f),
            textSecondaryColor: new ColorRgba(0.7f, 0.7f, 0.7f, 1f)
        );

        public static ColorScheme LightTheme => new ColorScheme(
            tileEmptyColor: new ColorRgba(0.9f, 0.9f, 0.9f, 1f),
            tileFilledColor: new ColorRgba(0.8f, 0.8f, 0.8f, 1f),
            tileAbsentColor: new ColorRgba(0.47f, 0.47f, 0.49f, 1f),
            tilePresentColor: new ColorRgba(0.79f, 0.73f, 0.42f, 1f),
            tileCorrectColor: new ColorRgba(0.42f, 0.68f, 0.39f, 1f),
            tileTextColor: new ColorRgba(0.1f, 0.1f, 0.1f, 1f),
            keyDefaultColor: new ColorRgba(0.82f, 0.82f, 0.84f, 1f),
            keyAbsentColor: new ColorRgba(0.47f, 0.47f, 0.49f, 1f),
            keyPresentColor: new ColorRgba(0.79f, 0.73f, 0.42f, 1f),
            keyCorrectColor: new ColorRgba(0.42f, 0.68f, 0.39f, 1f),
            keyTextColor: new ColorRgba(0.1f, 0.1f, 0.1f, 1f),
            backgroundColor: new ColorRgba(0.98f, 0.98f, 0.98f, 1f),
            panelColor: new ColorRgba(1f, 1f, 1f, 1f),
            textPrimaryColor: new ColorRgba(0.1f, 0.1f, 0.1f, 1f),
            textSecondaryColor: new ColorRgba(0.4f, 0.4f, 0.4f, 1f)
        );
    }
}
