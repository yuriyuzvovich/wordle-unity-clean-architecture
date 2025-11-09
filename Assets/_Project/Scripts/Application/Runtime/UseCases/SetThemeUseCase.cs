using System;
using Wordle.Application.Interfaces;
using Wordle.Core.Theme;

namespace Wordle.Application.UseCases
{
    public class SetThemeUseCase
    {
        private readonly IThemeService _themeService;
        private readonly ILogService _logService;

        public SetThemeUseCase(IThemeService themeService, ILogService logService)
        {
            _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        public void Execute(ThemeType themeType)
        {
            _logService.LogInfo($"SetThemeUseCase: Changing theme to {themeType}");
            _themeService.SetTheme(themeType);
        }
    }
}
