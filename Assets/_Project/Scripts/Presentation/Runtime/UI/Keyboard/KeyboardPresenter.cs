using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Wordle.Application.Events;
using Wordle.Application.Interfaces;
using Wordle.Core.Events;
using Wordle.Core.Interfaces;
using Wordle.Presentation.Inputs;

namespace Wordle.Presentation.UI.Keyboard
{
    public class KeyboardPresenter : IDisposable
    {
        private readonly IKeyboardView _view;
        private readonly IEventBus _eventBus;
        private readonly IGameStateRepository _gameStateRepository;
        private readonly ILocalizationService _localizationService;
        private readonly IKeyboardLayoutProvider _layoutProvider;
        private readonly TouchInputService _touchInputService;
        private readonly ILogService _logService;

        public KeyboardPresenter(
            IKeyboardView view,
            IEventBus eventBus,
            IGameStateRepository gameStateRepository,
            ILocalizationService localizationService,
            IKeyboardLayoutProvider layoutProvider,
            TouchInputService touchInputService,
            ILogService logService
        )
        {
            _view = view;
            _eventBus = eventBus;
            _gameStateRepository = gameStateRepository;
            _localizationService = localizationService;
            _layoutProvider = layoutProvider;
            _touchInputService = touchInputService;
            _logService = logService;

            SubscribeToEvents();
        }

        public void Initialize()
        {
            var layout = _layoutProvider.GetCurrentLanguageLayout();
            CreateKeyboardLayout(layout);
            UpdateLocalization();
        }

        public async UniTask RestoreKeyStatesAsync()
        {
            var gameState = await _gameStateRepository.LoadAsync();
            if (gameState == null || !gameState.HasGuesses())
            {
                return;
            }

            foreach (var guess in gameState.Guesses)
            {
                foreach (var letterPos in guess.Evaluations)
                {
                    var keyLabel = letterPos.Letter.ToString();
                    _view.UpdateKeyState(keyLabel, letterPos.Evaluation);
                }
            }
        }

        private void CreateKeyboardLayout(Application.DTOs.KeyboardLayoutConfig layout)
        {
            for (int i = 0; i < layout.Row1Keys.Length; i++)
            {
                _view.CreateKey(layout.Row1Keys[i], KeyType.Letter, 0, OnKeyPressed);
            }
            _view.CreateKey("BACKSPACE", KeyType.Backspace, 0, OnKeyPressed);

            for (int i = 0; i < layout.Row2Keys.Length; i++)
            {
                _view.CreateKey(layout.Row2Keys[i], KeyType.Letter, 1, OnKeyPressed);
            }

            for (int i = 0; i < layout.Row3Keys.Length; i++)
            {
                _view.CreateKey(layout.Row3Keys[i], KeyType.Letter, 2, OnKeyPressed);
            }
            _view.CreateKey("ENTER", KeyType.Enter, 2, OnKeyPressed);
        }

        private void SubscribeToEvents()
        {
            _eventBus.Subscribe<GuessSubmittedEvent>(OnGuessSubmitted);
            _eventBus.Subscribe<GameStartedEvent>(OnGameStarted);
            _eventBus.Subscribe<LanguageChangedEvent>(OnLanguageChanged);
        }

        private void OnGuessSubmitted(GuessSubmittedEvent evt)
        {
            UpdateKeyStatesFromGuessAsync().Forget();
        }

        private void OnGameStarted(GameStartedEvent evt)
        {
            _view.ResetAllKeys();
        }

        private void OnLanguageChanged(LanguageChangedEvent evt)
        {
            ReconfigureKeyboardAsync().Forget();
        }

        private async UniTaskVoid ReconfigureKeyboardAsync()
        {
            _view.ClearAllKeys();

            var layout = _layoutProvider.GetCurrentLanguageLayout();
            CreateKeyboardLayout(layout);
            UpdateLocalization();

            await RestoreKeyStatesAsync();
        }

        private async UniTaskVoid UpdateKeyStatesFromGuessAsync()
        {
            var gameState = await _gameStateRepository.LoadAsync();
            if (gameState == null || !gameState.HasGuesses())
            {
                return;
            }

            var lastGuess = gameState.GetLastGuess();

            foreach (var letterPos in lastGuess.Evaluations)
            {
                var keyLabel = letterPos.Letter.ToString();
                _view.UpdateKeyState(keyLabel, letterPos.Evaluation);
            }
        }

        private void UpdateLocalization()
        {
            var enterText = _localizationService.GetString("keyboard.enter");
            var backspaceText = _localizationService.GetString("keyboard.backspace");
            _view.UpdateKeyLocalization(enterText, backspaceText);
        }

        private void OnKeyPressed(string keyLabel)
        {
            if (_touchInputService == null)
            {
                _logService.LogError($"KeyboardPresenter: Cannot process key '{keyLabel}' - TouchInputService is null!");
                return;
            }

            if (keyLabel == "ENTER")
            {
                _touchInputService.PressEnter();
            }
            else if (keyLabel == "BACKSPACE")
            {
                _touchInputService.PressBackspace();
            }
            else if (keyLabel.Length == 1)
            {
                _touchInputService.PressLetter(keyLabel[0]);
            }
        }

        public void Dispose()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<GuessSubmittedEvent>(OnGuessSubmitted);
                _eventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
                _eventBus.Unsubscribe<LanguageChangedEvent>(OnLanguageChanged);
            }
        }
    }
}