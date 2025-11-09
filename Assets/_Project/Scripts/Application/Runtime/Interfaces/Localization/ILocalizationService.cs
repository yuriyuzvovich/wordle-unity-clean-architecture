using System.Collections.Generic;

namespace Wordle.Application.Interfaces
{
    /// <summary>
    /// Service for retrieving localized text strings.
    /// Supports parameterized strings for dynamic content.
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Gets the current language code (e.g., "en", "es", "fr").
        /// </summary>
        string CurrentLanguage { get; }

        /// <summary>
        /// Gets all available language codes.
        /// </summary>
        IReadOnlyList<string> AvailableLanguages { get; }

        /// <summary>
        /// Gets a localized string by key.
        /// </summary>
        /// <param name="key">The localization key (e.g., "keyboard.enter")</param>
        /// <returns>The localized string, or the key if not found</returns>
        string GetString(string key);

        /// <summary>
        /// Gets a localized string with parameter substitution.
        /// </summary>
        /// <param name="key">The localization key</param>
        /// <param name="parameters">Parameters to substitute in the string (e.g., {word}, {attempts})</param>
        /// <returns>The localized string with parameters replaced</returns>
        string GetString(string key, Dictionary<string, string> parameters);

        /// <summary>
        /// Sets the current language and reloads localized strings.
        /// </summary>
        /// <param name="languageCode">Language code to set (e.g., "en", "es")</param>
        void SetLanguage(string languageCode);

        /// <summary>
        /// Gets the display name of a language (e.g., "English" for "en").
        /// </summary>
        /// <param name="languageCode">The language code</param>
        /// <returns>The display name of the language</returns>
        string GetLanguageDisplayName(string languageCode);
    }
}
