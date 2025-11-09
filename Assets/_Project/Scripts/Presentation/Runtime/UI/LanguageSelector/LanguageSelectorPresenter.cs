using System;
using System.Collections.Generic;
using UnityEngine;
using Wordle.Application.Events;
using Wordle.Application.Interfaces;
using Wordle.Presentation.UI.Theme;

namespace Wordle.Presentation.UI.LanguageSelector
{
    /// <summary>
    /// Presenter for language selector logic.
    /// Handles dropdown initialization, language changes, and theme coordination.
    /// </summary>
    public class LanguageSelectorPresenter : IDisposable
    {
        private readonly ILanguageSelectorView _view;
        private readonly ILocalizationService _localizationService;
        private readonly IEventBus _eventBus;
        private readonly IThemeService _themeService;

        private Color _textColor;
        private Color _backgroundColor;

        public LanguageSelectorPresenter(
            ILanguageSelectorView view,
            ILocalizationService localizationService,
            IEventBus eventBus,
            IThemeService themeService)
        {
            _view = view;
            _localizationService = localizationService;
            _eventBus = eventBus;
            _themeService = themeService;

            SubscribeToEvents();
            LoadThemeColors();
        }

        public void Initialize()
        {
            ApplyThemeColors();
            InitializeDropdown();
        }

        public void Dispose()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<ThemeChangedEvent>(OnThemeChanged);
                _eventBus.Unsubscribe<LanguageChangedEvent>(OnLanguageUpdated);
            }
        }

        private void SubscribeToEvents()
        {
            _eventBus.Subscribe<ThemeChangedEvent>(OnThemeChanged);
            _eventBus.Subscribe<LanguageChangedEvent>(OnLanguageUpdated);
        }

        private void OnLanguageUpdated(LanguageChangedEvent evt)
        {
            InitializeDropdown();
        }

        public void OnLanguageChanged(int index)
        {
            if (index < 0 || index >= _localizationService.AvailableLanguages.Count)
            {
                return;
            }

            var languageCode = _localizationService.AvailableLanguages[index];
            _localizationService.SetLanguage(languageCode);
        }

        private void InitializeDropdown()
        {
            var options = new List<string>();
            var currentLanguage = _localizationService.CurrentLanguage;
            var currentIndex = 0;

            for (int i = 0; i < _localizationService.AvailableLanguages.Count; i++)
            {
                var languageCode = _localizationService.AvailableLanguages[i];
                var displayName = _localizationService.GetLanguageDisplayName(languageCode);
                options.Add(displayName);

                if (languageCode == currentLanguage)
                {
                    currentIndex = i;
                }
            }

            _view.SetDropdownOptions(options);
            _view.SetSelectedIndex(currentIndex);
        }

        private void OnThemeChanged(ThemeChangedEvent evt)
        {
            LoadThemeColors();
            ApplyThemeColors();
        }

        private void ApplyThemeColors()
        {
            _view.SetTextColor(_textColor);
            _view.SetBackgroundColor(_backgroundColor);
        }

        private void LoadThemeColors()
        {
            var colorScheme = _themeService.CurrentColorScheme;
            _textColor = colorScheme.TextPrimaryColor.ToUnityColor();
            _backgroundColor = colorScheme.PanelColor.ToUnityColor();
        }
    }
}