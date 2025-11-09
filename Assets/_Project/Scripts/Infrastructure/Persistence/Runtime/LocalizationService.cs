using System;
using System.Collections.Generic;
using UnityEngine;
using Wordle.Application.Events;
using Wordle.Application.Interfaces;

namespace Wordle.Infrastructure.Persistence
{
    public class LocalizationService : ILocalizationService
    {
        private readonly ILogService _logService;
        private readonly ILanguageRepository _languageRepository;
        private readonly IEventBus _eventBus;

        private Dictionary<string, string> _currentStrings = new Dictionary<string, string>();
        private string _currentLanguage = "en";
        private readonly List<string> _availableLanguages = new List<string> { "en", "ru", "de" };
        private readonly Dictionary<string, string> _languageDisplayNames = new Dictionary<string, string> {
            { "en", "EN" },
            { "ru", "RU" },
            { "de", "DE" },
        };

        public string CurrentLanguage => _currentLanguage;
        public IReadOnlyList<string> AvailableLanguages => _availableLanguages;

        public LocalizationService(ILogService logService, ILanguageRepository languageRepository, IEventBus eventBus)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            _languageRepository = languageRepository ?? throw new ArgumentNullException(nameof(languageRepository));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));

            var savedLanguage = _languageRepository.GetLanguage();
            if (!string.IsNullOrEmpty(savedLanguage) && _availableLanguages.Contains(savedLanguage))
            {
                _currentLanguage = savedLanguage;
            }

            LoadLanguageData(_currentLanguage);
        }

        public string GetString(string key)
        {
            if (_currentStrings.TryGetValue(key, out var value))
            {
                return value;
            }

            _logService.LogWarning($"Localization key not found: {key}");
            return key;
        }

        public string GetString(string key, Dictionary<string, string> parameters)
        {
            var template = GetString(key);

            if (parameters == null || parameters.Count == 0)
            {
                return template;
            }

            var result = template;
            foreach (var param in parameters)
            {
                result = result.Replace($"{{{param.Key}}}", param.Value);
            }

            return result;
        }

        public void SetLanguage(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
            {
                _logService.LogWarning("Attempted to set null or empty language code");
                return;
            }

            if (!_availableLanguages.Contains(languageCode))
            {
                _logService.LogWarning($"Language code not supported: {languageCode}");
                return;
            }

            if (_currentLanguage == languageCode)
            {
                _logService.LogInfo($"Language already set to: {languageCode}");
                return;
            }

            _currentLanguage = languageCode;
            LoadLanguageData(languageCode);
            _languageRepository.SaveLanguage(languageCode);
            _eventBus.Publish(new LanguageChangedEvent(languageCode));

            _logService.LogInfo($"Language changed to: {languageCode}");
        }

        public string GetLanguageDisplayName(string languageCode)
        {
            if (_languageDisplayNames.TryGetValue(languageCode, out var displayName))
            {
                return displayName;
            }

            return languageCode;
        }

        private void LoadLanguageData(string languageCode)
        {
            try
            {
                var path = $"Localization/{languageCode}";
                var textAsset = Resources.Load<TextAsset>(path);

                if (textAsset == null)
                {
                    _logService.LogError($"Localization file not found: {path}");
                    LoadFallbackLanguage();
                    return;
                }

                var localizationData = JsonUtility.FromJson<LocalizationData>(textAsset.text);

                if (localizationData == null)
                {
                    _logService.LogError($"Failed to parse localization data: {path}");
                    LoadFallbackLanguage();
                    return;
                }

                _currentStrings = localizationData.ToDictionary();
                _logService.LogInfo($"Loaded {_currentStrings.Count} localization strings for language: {languageCode}");
            }
            catch (Exception ex)
            {
                _logService.LogError($"Failed to load localization data for {languageCode}: {ex.Message}");
                LoadFallbackLanguage();
            }
        }

        private void LoadFallbackLanguage()
        {
            if (_currentLanguage == "en")
            {
                _logService.LogError("Failed to load fallback language (English)");
                _currentStrings.Clear();
                return;
            }

            _logService.LogWarning("Loading fallback language: English");
            LoadLanguageData("en");
        }
    }
}