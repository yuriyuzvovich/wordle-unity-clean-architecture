using Wordle.Application.Interfaces;
using Wordle.Core.Theme;

namespace Wordle.Application.Tests.Mocks
{
    public class MockThemeService : IThemeService
    {
        private ThemeType _currentTheme = ThemeType.Light;
        private ColorScheme _currentColorScheme;

        public MockThemeService()
        {
            _currentColorScheme = ThemeColors.GetColorScheme(_currentTheme);
        }

        public ThemeType CurrentTheme => _currentTheme;
        public ColorScheme CurrentColorScheme => _currentColorScheme;

        public void SetTheme(ThemeType themeType)
        {
            _currentTheme = themeType;
            _currentColorScheme = ThemeColors.GetColorScheme(themeType);
        }
    }
}
