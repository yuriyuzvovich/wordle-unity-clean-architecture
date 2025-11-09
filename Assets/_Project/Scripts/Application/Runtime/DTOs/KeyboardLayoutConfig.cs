using System;

namespace Wordle.Application.DTOs
{
    /// <summary>
    /// Defines a keyboard layout for a specific language.
    /// Each row contains the letter keys to be displayed on that row.
    /// </summary>
    [Serializable]
    public class KeyboardLayoutConfig
    {
        public string LanguageCode;
        public string[] Row1Keys;
        public string[] Row2Keys;
        public string[] Row3Keys;

        public KeyboardLayoutConfig(string languageCode, string[] row1Keys, string[] row2Keys, string[] row3Keys)
        {
            LanguageCode = languageCode;
            Row1Keys = row1Keys;
            Row2Keys = row2Keys;
            Row3Keys = row3Keys;
        }

        /// <summary>
        /// Total number of letter keys (excluding ENTER and BACKSPACE).
        /// </summary>
        public int TotalKeys => Row1Keys.Length + Row2Keys.Length + Row3Keys.Length;

        /// <summary>
        /// Total number of all keys including ENTER and BACKSPACE.
        /// </summary>
        public int TotalKeysWithSpecial => TotalKeys + 2;
    }
}
