using System;
using System.Collections.Generic;
using Wordle.Application.DTOs;
using Wordle.Application.Interfaces;

namespace Wordle.Infrastructure.Configuration
{
    /// <summary>
    /// Provides keyboard layout configurations for different languages.
    /// Manages English QWERTY and German QWERTZ layouts.
    /// </summary>
    public class KeyboardLayoutProvider : IKeyboardLayoutProvider
    {
        private readonly ILocalizationService _localizationService;
        private readonly Dictionary<string, KeyboardLayoutConfig> _layouts;

        private const string DEFAULT_LANGUAGE = "en";

        private static readonly KeyboardLayoutConfig ENGLISH_LAYOUT = new KeyboardLayoutConfig(
            languageCode: "en",
            row1Keys: new[] { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P" },
            row2Keys: new[] { "A", "S", "D", "F", "G", "H", "J", "K", "L" },
            row3Keys: new[] { "Z", "X", "C", "V", "B", "N", "M" }
        );

        private static readonly KeyboardLayoutConfig GERMAN_LAYOUT = new KeyboardLayoutConfig(
            languageCode: "de",
            row1Keys: new[] { "Q", "W", "E", "R", "T", "Z", "U", "I", "O", "P", "Ü" },
            row2Keys: new[] { "A", "S", "D", "F", "G", "H", "J", "K", "L", "Ö", "Ä" },
            row3Keys: new[] { "Y", "X", "C", "V", "B", "N", "M", "ß" }
        );
        
        private static readonly KeyboardLayoutConfig RUSSIAN_LAYOUT = new KeyboardLayoutConfig(
            languageCode: "ru",
            row1Keys: new[] { "Й", "Ц", "У", "К", "Е", "Н", "Г", "Ш", "Щ", "З", "Х", "Ъ" },
            row2Keys: new[] { "Ф", "Ы", "В", "А", "П", "Р", "О", "Л", "Д", "Ж", "Э" },
            row3Keys: new[] { "Я", "Ч", "С", "М", "И", "Т", "Ь", "Б", "Ю" }
        );

        public KeyboardLayoutProvider(ILocalizationService localizationService)
        {
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));

            _layouts = new Dictionary<string, KeyboardLayoutConfig>
            {
                { "en", ENGLISH_LAYOUT },
                { "de", GERMAN_LAYOUT },
                { "ru", RUSSIAN_LAYOUT },
            };
        }

        public KeyboardLayoutConfig GetLayout(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
            {
                return _layouts[DEFAULT_LANGUAGE];
            }

            if (_layouts.TryGetValue(languageCode, out var layout))
            {
                return layout;
            }

            return _layouts[DEFAULT_LANGUAGE];
        }

        public KeyboardLayoutConfig GetCurrentLanguageLayout()
        {
            var currentLanguage = _localizationService.CurrentLanguage;
            return GetLayout(currentLanguage);
        }

        public bool HasLayout(string languageCode)
        {
            return !string.IsNullOrEmpty(languageCode) && _layouts.ContainsKey(languageCode);
        }
    }
}
