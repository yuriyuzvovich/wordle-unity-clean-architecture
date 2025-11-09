using System;

namespace Wordle.Presentation.Inputs
{
    /// <summary>
    /// Programmatic input service for touch/virtual keyboard.
    /// UI elements (like virtual keyboard buttons) call methods to trigger input events.
    /// </summary>
    public class TouchInputService : IInputService
    {
        public event Action<char> OnLetterPressed;
        public event Action OnBackspacePressed;
        public event Action OnEnterPressed;

        private bool _isEnabled = true;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        public void Initialize()
        {
            _isEnabled = true;
        }

        public void Shutdown()
        {
            _isEnabled = false;
            OnLetterPressed = null;
            OnBackspacePressed = null;
            OnEnterPressed = null;
        }

        public void PressLetter(char letter)
        {
            if (!_isEnabled)
            {
                return;
            }
            if (!char.IsLetter(letter))
            {
                return;
            }

            OnLetterPressed?.Invoke(char.ToUpper(letter));
        }

        public void PressBackspace()
        {
            if (!_isEnabled)
            {
                return;
            }

            OnBackspacePressed?.Invoke();
        }

        public void PressEnter()
        {
            if (!_isEnabled)
            {
                return;
            }

            OnEnterPressed?.Invoke();
        }
    }
}