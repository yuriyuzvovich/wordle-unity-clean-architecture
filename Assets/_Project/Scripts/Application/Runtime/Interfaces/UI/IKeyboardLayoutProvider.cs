using Wordle.Application.DTOs;

namespace Wordle.Application.Interfaces
{
    /// <summary>
    /// Provides keyboard layout configurations based on language.
    /// </summary>
    public interface IKeyboardLayoutProvider
    {
        /// <summary>
        /// Gets the keyboard layout for the specified language code.
        /// </summary>
        /// <param name="languageCode">The language code (e.g., "en", "de")</param>
        /// <returns>The keyboard layout configuration for the language</returns>
        KeyboardLayoutConfig GetLayout(string languageCode);

        /// <summary>
        /// Gets the keyboard layout for the current active language.
        /// </summary>
        /// <returns>The keyboard layout configuration for the current language</returns>
        KeyboardLayoutConfig GetCurrentLanguageLayout();

        /// <summary>
        /// Checks if a layout exists for the specified language code.
        /// </summary>
        /// <param name="languageCode">The language code to check</param>
        /// <returns>True if a layout exists, false otherwise</returns>
        bool HasLayout(string languageCode);
    }
}
