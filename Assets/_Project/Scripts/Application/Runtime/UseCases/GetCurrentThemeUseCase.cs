using System;
using Wordle.Application.DTOs;
using Wordle.Application.Interfaces;

namespace Wordle.Application.UseCases
{
    public class GetCurrentThemeUseCase
    {
        private readonly IThemeService _themeService;
        private readonly ILogService _logService;

        public GetCurrentThemeUseCase(IThemeService themeService, ILogService logService)
        {
            _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        public ThemeDto Execute()
        {
            return new ThemeDto(_themeService.CurrentTheme, _themeService.CurrentColorScheme);
        }
    }
}