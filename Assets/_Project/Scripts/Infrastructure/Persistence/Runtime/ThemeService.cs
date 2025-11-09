using System;
using Wordle.Application.Events;
using Wordle.Application.Interfaces;
using Wordle.Core.Theme;

namespace Wordle.Infrastructure.Persistence
{
    public class ThemeService : IThemeService
    {
        private readonly ILogService _logService;
        private readonly IThemeRepository _themeRepository;
        private readonly IEventBus _eventBus;

        private ThemeType _currentTheme;
        private ColorScheme _currentColorScheme;

        public ThemeType CurrentTheme => _currentTheme;
        public ColorScheme CurrentColorScheme => _currentColorScheme;

        public ThemeService(ILogService logService, IThemeRepository themeRepository, IEventBus eventBus)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            _themeRepository = themeRepository ?? throw new ArgumentNullException(nameof(themeRepository));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

            _currentTheme = _themeRepository.GetTheme();
            _currentColorScheme = ThemeColors.GetColorScheme(_currentTheme);

            _logService.LogInfo($"ThemeService initialized with theme: {_currentTheme}");
        }

        public void SetTheme(ThemeType themeType)
        {
            if (_currentTheme == themeType)
            {
                _logService.LogInfo($"Theme already set to: {themeType}");
                return;
            }

            _currentTheme = themeType;
            _currentColorScheme = ThemeColors.GetColorScheme(themeType);
            _themeRepository.SaveTheme(themeType);
            _eventBus.Publish(new ThemeChangedEvent(themeType, _currentColorScheme));

            _logService.LogInfo($"Theme changed to: {themeType}");
        }
    }
}
