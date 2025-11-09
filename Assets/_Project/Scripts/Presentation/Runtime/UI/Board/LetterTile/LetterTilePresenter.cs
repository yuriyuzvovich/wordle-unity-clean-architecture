using System;
using UnityEngine;
using Wordle.Application.Events;
using Wordle.Application.Interfaces;
using Wordle.Core.Events;
using Wordle.Core.ValueObjects;
using Wordle.Presentation.UI.Theme;

namespace Wordle.Presentation.UI.Board
{
    /// <summary>
    /// Presenter for letter tile logic.
    /// Handles state management, event filtering, theme coordination, and evaluation mapping.
    /// </summary>
    public class LetterTilePresenter : IDisposable
    {
        private readonly ILetterTileView _view;
        private readonly IEventBus _eventBus;
        private readonly IThemeService _themeService;

        private int _row;
        private int _column;
        private TileState _currentState = TileState.Empty;
        private char _currentLetter = '\0';

        private Color _emptyColor;
        private Color _filledColor;
        private Color _absentColor;
        private Color _presentColor;
        private Color _correctColor;
        private Color _textColor;

        public LetterTilePresenter(ILetterTileView view, IEventBus eventBus, IThemeService themeService)
        {
            _view = view;
            _eventBus = eventBus;
            _themeService = themeService;

            SubscribeToEvents();
            LoadThemeColors();
        }

        public void Initialize(int row, int column)
        {
            _row = row;
            _column = column;

            _view.SetTextColor(_textColor);
            UpdateVisualState(_currentState);
        }

        public void SetLetter(char letter)
        {
            _currentLetter = char.ToUpper(letter);
            _view.SetText(_currentLetter.ToString());
            _currentState = TileState.Filled;
            UpdateVisualState(_currentState);
        }

        public void ClearLetter()
        {
            _currentLetter = '\0';
            _view.SetText(string.Empty);
            _currentState = TileState.Empty;
            UpdateVisualState(_currentState);
        }

        public void SetEvaluation(LetterEvaluation evaluation)
        {
            _currentState = evaluation switch
            {
                LetterEvaluation.Correct => TileState.Correct,
                LetterEvaluation.Present => TileState.Present,
                LetterEvaluation.Absent => TileState.Absent,
                _ => _currentState
            };

            _view.PlayFlipAnimation(() => UpdateVisualState(_currentState));
        }

        public void ResetTile()
        {
            _currentLetter = '\0';
            _view.SetText(string.Empty);
            _currentState = TileState.Empty;
            UpdateVisualState(_currentState);
            _view.ResetVisuals();
        }

        public void PlayShakeAnimation()
        {
            _view.PlayShakeAnimation();
        }

        private void UpdateVisualState(TileState state)
        {
            Color backgroundColor = state switch
            {
                TileState.Empty => _emptyColor,
                TileState.Filled => _filledColor,
                TileState.Absent => _absentColor,
                TileState.Present => _presentColor,
                TileState.Correct => _correctColor,
                _ => _emptyColor
            };

            _view.SetBackgroundColor(backgroundColor);
        }

        private void LoadThemeColors()
        {
            var colorScheme = _themeService.CurrentColorScheme;
            _emptyColor = colorScheme.TileEmptyColor.ToUnityColor();
            _filledColor = colorScheme.TileFilledColor.ToUnityColor();
            _absentColor = colorScheme.TileAbsentColor.ToUnityColor();
            _presentColor = colorScheme.TilePresentColor.ToUnityColor();
            _correctColor = colorScheme.TileCorrectColor.ToUnityColor();
            _textColor = colorScheme.TileTextColor.ToUnityColor();
        }

        private void SubscribeToEvents()
        {
            _eventBus.Subscribe<LetterEnteredEvent>(OnLetterEntered);
            _eventBus.Subscribe<LetterDeletedEvent>(OnLetterDeleted);
            _eventBus.Subscribe<GuessSubmittedEvent>(OnGuessSubmitted);
            _eventBus.Subscribe<GameStartedEvent>(OnGameStarted);
            _eventBus.Subscribe<ThemeChangedEvent>(OnThemeChanged);
        }

        private void OnLetterEntered(LetterEnteredEvent evt)
        {
            if (evt.Row != _row || evt.Position != _column) return;

            SetLetter(evt.Letter);
            _view.PlayPopAnimation();
        }

        private void OnLetterDeleted(LetterDeletedEvent evt)
        {
            if (evt.Row != _row || evt.Position != _column) return;

            ClearLetter();
        }

        private void OnGuessSubmitted(GuessSubmittedEvent evt)
        {
        }

        private void OnGameStarted(GameStartedEvent evt)
        {
            ResetTile();
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
                _eventBus.Unsubscribe<LetterEnteredEvent>(OnLetterEntered);
                _eventBus.Unsubscribe<LetterDeletedEvent>(OnLetterDeleted);
                _eventBus.Unsubscribe<GuessSubmittedEvent>(OnGuessSubmitted);
                _eventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
                _eventBus.Unsubscribe<ThemeChangedEvent>(OnThemeChanged);
            }
        }

        public int Row => _row;
        public int Column => _column;
        public char CurrentLetter => _currentLetter;
        public TileState CurrentState => _currentState;
    }
}
