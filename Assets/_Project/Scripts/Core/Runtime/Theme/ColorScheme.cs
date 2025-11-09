namespace Wordle.Core.Theme
{
    public class ColorScheme
    {
        public ColorRgba TileEmptyColor { get; }
        public ColorRgba TileFilledColor { get; }
        public ColorRgba TileAbsentColor { get; }
        public ColorRgba TilePresentColor { get; }
        public ColorRgba TileCorrectColor { get; }
        public ColorRgba TileTextColor { get; }

        public ColorRgba KeyDefaultColor { get; }
        public ColorRgba KeyAbsentColor { get; }
        public ColorRgba KeyPresentColor { get; }
        public ColorRgba KeyCorrectColor { get; }
        public ColorRgba KeyTextColor { get; }

        public ColorRgba BackgroundColor { get; }
        public ColorRgba PanelColor { get; }
        public ColorRgba TextPrimaryColor { get; }
        public ColorRgba TextSecondaryColor { get; }

        public ColorScheme(
            ColorRgba tileEmptyColor,
            ColorRgba tileFilledColor,
            ColorRgba tileAbsentColor,
            ColorRgba tilePresentColor,
            ColorRgba tileCorrectColor,
            ColorRgba tileTextColor,
            ColorRgba keyDefaultColor,
            ColorRgba keyAbsentColor,
            ColorRgba keyPresentColor,
            ColorRgba keyCorrectColor,
            ColorRgba keyTextColor,
            ColorRgba backgroundColor,
            ColorRgba panelColor,
            ColorRgba textPrimaryColor,
            ColorRgba textSecondaryColor)
        {
            TileEmptyColor = tileEmptyColor;
            TileFilledColor = tileFilledColor;
            TileAbsentColor = tileAbsentColor;
            TilePresentColor = tilePresentColor;
            TileCorrectColor = tileCorrectColor;
            TileTextColor = tileTextColor;

            KeyDefaultColor = keyDefaultColor;
            KeyAbsentColor = keyAbsentColor;
            KeyPresentColor = keyPresentColor;
            KeyCorrectColor = keyCorrectColor;
            KeyTextColor = keyTextColor;

            BackgroundColor = backgroundColor;
            PanelColor = panelColor;
            TextPrimaryColor = textPrimaryColor;
            TextSecondaryColor = textSecondaryColor;
        }
    }
}
