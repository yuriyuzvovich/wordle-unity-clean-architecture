using System;
using UnityEngine;
using Wordle.Application.Interfaces;

namespace Wordle.Presentation.Inputs
{
    /// <summary>
    /// Detects keyboard input and fires events for letter entry, deletion, and submission.
    /// Respects OS keyboard layout (QWERTY, QWERTZ, AZERTY, etc.) for character input.
    /// Uses IEngineLifecycle to poll input every frame without being a MonoBehaviour.
    /// </summary>
    public class KeyboardInputService : IInputService
    {
        public event Action<char> OnLetterPressed;
        public event Action OnBackspacePressed;
        public event Action OnEnterPressed;

        private readonly IEngineLifecycle _lifecycle;
        private bool _isEnabled = true;

        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }

        public KeyboardInputService(IEngineLifecycle lifecycle)
        {
            _lifecycle = lifecycle;
        }

        public void Initialize()
        {
            _isEnabled = true;
            _lifecycle.OnFrameTick += HandleFrameTick;
        }

        public void Shutdown()
        {
            _isEnabled = false;
            _lifecycle.OnFrameTick -= HandleFrameTick;
            OnLetterPressed = null;
            OnBackspacePressed = null;
            OnEnterPressed = null;
        }

        private void HandleFrameTick()
        {
            if (!_isEnabled)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnEnterPressed?.Invoke();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                OnBackspacePressed?.Invoke();
                return;
            }

            string input = Input.inputString;
            if (!string.IsNullOrEmpty(input))
            {
                foreach (char c in input)
                {
                    if (char.IsLetter(c))
                    {
                        char upperLetter = char.ToUpper(c);
                        OnLetterPressed?.Invoke(upperLetter);
                        return;
                    }
                }
            }
        }
    }
}