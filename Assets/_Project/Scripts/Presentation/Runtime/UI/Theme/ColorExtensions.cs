using UnityEngine;
using Wordle.Core.Theme;

namespace Wordle.Presentation.UI.Theme
{
    public static class ColorExtensions
    {
        public static Color ToUnityColor(this ColorRgba colorRgba)
        {
            return new Color(colorRgba.R, colorRgba.G, colorRgba.B, colorRgba.A);
        }

        public static ColorRgba ToColorRgba(this Color color)
        {
            return new ColorRgba(color.r, color.g, color.b, color.a);
        }
    }
}
