using System;
using UnityEngine;
using Wordle.Application.Events;
using Wordle.Application.Interfaces;
using Wordle.Core.ValueObjects;
using Wordle.Presentation.UI.Theme;

namespace Wordle.Presentation.UI.Keyboard
{
    /// <summary>
    /// Presenter for keyboard key logic.
    /// Handles state management, text mapping, theme coordination, and user interactions.
    /// </summary>
    public class KeyPresenter : IDisposable
    {
        private readonly IKeyView _view;
        private readonly IEventBus _eventBus;
        private readonly IThemeService _themeService;

        private string _keyLabel;
        private KeyType _keyType;
        private KeyState _currentState = KeyState.Default;
        private Action<string> _onKeyPressed;
        private string _enterText = "⏎";
        private string _backspaceText = "←";

        private Color _defaultColor;
        private Color _absentColor;
        private Color _presentColor;
        private Color _correctColor;
        private Color _textColor;

        public string KeyLabel => _keyLabel;
        public KeyType Type => _keyType;
        public KeyState CurrentState => _currentState;

        public KeyPresenter(IKeyView view, IEventBus eventBus, IThemeService themeService)
        {
            _view = view;
            _eventBus = eventBus;
            _themeService = themeService;

            SubscribeToEvents();
            LoadThemeColors();
        }

        public void Initialize(string keyLabel, KeyType keyType, Action<string> onKeyPressed)
        {
            _keyLabel = keyLabel;
            _keyType = keyType;
            _onKeyPressed = onKeyPressed;

            UpdateKeyText();
            UpdateVisualState(_currentState);
            _view.SetTextColor(_textColor);
        }

        public void SetLocalizedTexts(string enterText, string backspaceText)
        {
            _enterText = enterText;
            _backspaceText = backspaceText;
            UpdateKeyText();
        }

        public void SetKeyState(LetterEvaluation evaluation)
        {
            var newState = evaluation switch {
                LetterEvaluation.Correct => KeyState.Correct,
                LetterEvaluation.Present => KeyState.Present,
                LetterEvaluation.Absent => KeyState.Absent,
                _ => _currentState
            };

            if (newState > _currentState)
            {
                _currentState = newState;
                UpdateVisualState(_currentState);
            }
        }

        public void ResetKeyState()
        {
            _currentState = KeyState.Default;
            UpdateVisualState(_currentState);
        }

        public void OnButtonClicked()
        {
            _view.PlayPressAnimation();
            _onKeyPressed?.Invoke(_keyLabel);
        }

        public void ClearCallback()
        {
            _onKeyPressed = null;
        }

        private void UpdateKeyText()
        {
            string text = _keyType switch {
                KeyType.Enter => _enterText,
                KeyType.Backspace => _backspaceText,
                KeyType.Letter => _keyLabel,
                _ => _keyLabel
            };

            _view.SetText(text);
        }

        private void UpdateVisualState(KeyState state)
        {
            Color backgroundColor = state switch {
                KeyState.Default => _defaultColor,
                KeyState.Absent => _absentColor,
                KeyState.Present => _presentColor,
                KeyState.Correct => _correctColor,
                _ => _defaultColor
            };

            _view.SetBackgroundColor(backgroundColor);
        }

        private void LoadThemeColors()
        {
            var colorScheme = _themeService.CurrentColorScheme;
            _defaultColor = colorScheme.KeyDefaultColor.ToUnityColor();
            _absentColor = colorScheme.KeyAbsentColor.ToUnityColor();
            _presentColor = colorScheme.KeyPresentColor.ToUnityColor();
            _correctColor = colorScheme.KeyCorrectColor.ToUnityColor();
            _textColor = colorScheme.KeyTextColor.ToUnityColor();
        }

        private void SubscribeToEvents()
        {
            _eventBus.Subscribe<ThemeChangedEvent>(OnThemeChanged);
        }

        private void OnThemeChanged(ThemeChangedEvent evt)
        {
            LoadThemeColors();
            _view.SetTextColor(_textColor);
            UpdateVisualState(_currentState);
        }

        public void Dispose()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<ThemeChangedEvent>(OnThemeChanged);
            }
        }
    }
}