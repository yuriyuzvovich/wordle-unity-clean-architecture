using System;
using Wordle.Core.ValueObjects;

namespace Wordle.Presentation.UI.Keyboard
{
    /// <summary>
    /// View interface for keyboard rendering.
    /// Defines contract between KeyboardPresenter and KeyboardView.
    /// </summary>
    public interface IKeyboardView
    {
        /// <summary>
        /// Creates a key with the specified label and type in the given row.
        /// </summary>
        void CreateKey(string label, KeyType type, int rowIndex, Action<string> onKeyPressedCallback);

        /// <summary>
        /// Updates the state of a key based on letter evaluation.
        /// </summary>
        void UpdateKeyState(string label, LetterEvaluation evaluation);

        /// <summary>
        /// Updates localization for Enter and Backspace keys.
        /// </summary>
        void UpdateKeyLocalization(string enterText, string backspaceText);

        /// <summary>
        /// Resets all keys to default state.
        /// </summary>
        void ResetAllKeys();

        /// <summary>
        /// Clears all keys from the keyboard (returns to pool).
        /// </summary>
        void ClearAllKeys();

        /// <summary>
        /// Gets a key by its label.
        /// </summary>
        KeyView GetKey(string label);
    }
}
