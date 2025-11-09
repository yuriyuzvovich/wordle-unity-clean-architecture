using Wordle.Core.Theme;

namespace Wordle.Application.Interfaces
{
    public interface IThemeRepository
    {
        ThemeType GetTheme();
        void SaveTheme(ThemeType themeType);
    }
}
