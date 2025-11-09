using System;

namespace Wordle.Presentation.Inputs
{
    /// <summary>
    /// Interface for input detection services.
    /// Provides events for letter entry, deletion, and submission.
    /// </summary>
    public interface IInputService
    {
        event Action<char> OnLetterPressed;
        event Action OnBackspacePressed;
        event Action OnEnterPressed;

        bool IsEnabled { get; set; }

        void Initialize();
        void Shutdown();
    }
}
