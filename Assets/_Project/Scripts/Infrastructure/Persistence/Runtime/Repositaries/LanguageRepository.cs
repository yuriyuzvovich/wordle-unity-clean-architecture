using System;
using Wordle.Application.Interfaces;

namespace Wordle.Infrastructure.Persistence
{
    public class LanguageRepository : ILanguageRepository
    {
        private const string LanguagePrefsKey = "UserLanguage";
        private readonly ILogService _logService;
        private readonly ILocalStorageService _localStorageService;

        public LanguageRepository(ILogService logService, ILocalStorageService localStorageService)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            _localStorageService = localStorageService ?? throw new ArgumentNullException(nameof(localStorageService));
        }

        public string GetLanguage()
        {
            try
            {
                if (!_localStorageService.HasKey(LanguagePrefsKey))
                {
                    _logService.LogInfo("No saved language preference found");
                    return null;
                }

                var languageCode = _localStorageService.GetString(LanguagePrefsKey);
                _logService.LogInfo($"Loaded language preference: {languageCode}");
                return languageCode;
            }
            catch (Exception ex)
            {
                _logService.LogError($"Failed to load language preference: {ex.Message}");
                return null;
            }
        }

        public void SaveLanguage(string languageCode)
        {
            try
            {
                if (string.IsNullOrEmpty(languageCode))
                {
                    _logService.LogWarning("Attempted to save null or empty language code");
                    return;
                }

                _localStorageService.SetString(LanguagePrefsKey, languageCode);
                _localStorageService.Save();
                _logService.LogInfo($"Saved language preference: {languageCode}");
            }
            catch (Exception ex)
            {
                _logService.LogError($"Failed to save language preference: {ex.Message}");
                throw;
            }
        }
    }
}
