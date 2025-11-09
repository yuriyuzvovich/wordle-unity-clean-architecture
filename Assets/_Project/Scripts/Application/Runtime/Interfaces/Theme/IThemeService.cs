using Wordle.Core.Theme;

namespace Wordle.Application.Interfaces
{
    public interface IThemeService
    {
        ThemeType CurrentTheme { get; }
        ColorScheme CurrentColorScheme { get; }
        void SetTheme(ThemeType themeType);
    }
}
