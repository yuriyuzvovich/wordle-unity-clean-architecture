using System;
using Wordle.Application.Interfaces;
using Wordle.Core.Theme;

namespace Wordle.Infrastructure.Persistence
{
    public class ThemeRepository : IThemeRepository
    {
        private const string ThemePrefsKey = "UserTheme";
        private readonly ILogService _logService;
        private readonly ILocalStorageService _localStorageService;

        public ThemeRepository(ILogService logService, ILocalStorageService localStorageService)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            _localStorageService = localStorageService ?? throw new ArgumentNullException(nameof(localStorageService));
        }

        public ThemeType GetTheme()
        {
            try
            {
                if (!_localStorageService.HasKey(ThemePrefsKey))
                {
                    _logService.LogInfo("No saved theme preference found, defaulting to Light");
                    return ThemeType.Light;
                }

                var themeString = _localStorageService.GetString(ThemePrefsKey);
                if (Enum.TryParse<ThemeType>(themeString, out var themeType))
                {
                    _logService.LogInfo($"Loaded theme preference: {themeType}");
                    return themeType;
                }

                _logService.LogWarning($"Invalid theme value '{themeString}', defaulting to Light");
                return ThemeType.Light;
            }
            catch (Exception ex)
            {
                _logService.LogError($"Failed to load theme preference: {ex.Message}");
                return ThemeType.Light;
            }
        }

        public void SaveTheme(ThemeType themeType)
        {
            try
            {
                _localStorageService.SetString(ThemePrefsKey, themeType.ToString());
                _localStorageService.Save();
                _logService.LogInfo($"Saved theme preference: {themeType}");
            }
            catch (Exception ex)
            {
                _logService.LogError($"Failed to save theme preference: {ex.Message}");
                throw;
            }
        }
    }
}
