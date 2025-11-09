namespace Wordle.Application.Interfaces
{
    /// <summary>
    /// Repository for persisting user's language preference.
    /// </summary>
    public interface ILanguageRepository
    {
        /// <summary>
        /// Gets the saved language code.
        /// </summary>
        /// <returns>The language code, or null if not set</returns>
        string GetLanguage();

        /// <summary>
        /// Saves the language code.
        /// </summary>
        /// <param name="languageCode">The language code to save</param>
        void SaveLanguage(string languageCode);
    }
}
