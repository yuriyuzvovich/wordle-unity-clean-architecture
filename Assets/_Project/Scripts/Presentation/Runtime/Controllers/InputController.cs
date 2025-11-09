using System;
using System.Threading;
using UnityEngine;
using Wordle.Application.Commands;
using Wordle.Application.Interfaces;
using Wordle.Application.UseCases;
using Wordle.Core.Events;
using Wordle.Core.Interfaces;
using Wordle.Infrastructure.Common.DI;
using Cysharp.Threading.Tasks;
using Wordle.Presentation.Inputs;

namespace Wordle.Presentation.Controllers
{
    /// <summary>
    /// Manages input services and translates input into commands.
    /// Tracks the current word being entered and coordinates with the game state.
    /// </summary>
    public class InputController : IDisposable
    {
        private readonly ICommandQueue _commandQueue;
        private readonly IEventBus _eventBus;
        private readonly IGameStateRepository _gameStateRepository;
        private readonly IEngineLifecycle _lifecycle;

        private KeyboardInputService _keyboardInput;
        private TouchInputService _touchInput;
        private char[] _currentWord;
        private int _currentRow;
        private int _currentPosition;
        private bool _isProcessingGuess;
        private CancellationTokenSource _cts;

        public InputController(
            ICommandQueue commandQueue,
            IEventBus eventBus,
            IGameStateRepository gameStateRepository,
            IEngineLifecycle lifecycle
        )
        {
            _commandQueue = commandQueue ?? throw new ArgumentNullException(nameof(commandQueue));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _gameStateRepository = gameStateRepository ?? throw new ArgumentNullException(nameof(gameStateRepository));
            _lifecycle = lifecycle ?? throw new ArgumentNullException(nameof(lifecycle));
        }

        public void Initialize()
        {
            _currentWord = new char[5];
            _currentRow = 0;
            _currentPosition = 0;
            _isProcessingGuess = false;
            _cts = new CancellationTokenSource();

            CreateInputServices();
            InitializeInputServices();
            SubscribeToEvents();
        }

        private void CreateInputServices()
        {
            _keyboardInput = new KeyboardInputService(_lifecycle);
            _touchInput = new TouchInputService();
        }

        private void InitializeInputServices()
        {
            if (_keyboardInput != null)
            {
                _keyboardInput.Initialize();
                _keyboardInput.OnLetterPressed += HandleLetterPressed;
                _keyboardInput.OnBackspacePressed += HandleBackspacePressed;
                _keyboardInput.OnEnterPressed += HandleEnterPressed;
            }

            if (_touchInput != null)
            {
                _touchInput.Initialize();
                _touchInput.OnLetterPressed += HandleLetterPressed;
                _touchInput.OnBackspacePressed += HandleBackspacePressed;
                _touchInput.OnEnterPressed += HandleEnterPressed;
            }
        }

        private void SubscribeToEvents()
        {
            _eventBus.Subscribe<GuessSubmittedEvent>(OnGuessSubmitted);
            _eventBus.Subscribe<InvalidWordSubmittedEvent>(OnInvalidWordSubmitted);
            _eventBus.Subscribe<GameStartedEvent>(OnGameStarted);
        }

        private void HandleLetterPressed(char letter)
        {
            if (_isProcessingGuess)
            {
                return;
            }
            if (_currentPosition >= 5)
            {
                return;
            }

            _currentWord[_currentPosition] = letter;

            var command = new EnterLetterCommand(_eventBus, letter, _currentRow, _currentPosition);
            _commandQueue.Enqueue(command);

            _currentPosition++;
        }

        private void HandleBackspacePressed()
        {
            if (_isProcessingGuess)
            {
                return;
            }

            if (_currentPosition <= 0)
            {
                return;
            }

            _currentPosition--;
            _currentWord[_currentPosition] = '\0';

            var command = new DeleteLetterCommand(_eventBus, _currentRow, _currentPosition);
            _commandQueue.Enqueue(command);
        }

        private void HandleEnterPressed()
        {
            if (_isProcessingGuess)
            {
                return;
            }
            if (_currentPosition < 5)
            {
                return;
            }

            string guessWord = new string(_currentWord);
            SubmitGuessAsync(guessWord).Forget();
        }

        private async UniTaskVoid SubmitGuessAsync(string guessWord)
        {
            _isProcessingGuess = true;

            var gameState = await _gameStateRepository.LoadAsync();
            if (gameState == null)
            {
                Debug.LogError("InputController: Failed to load game state - no active game found");
                _isProcessingGuess = false;
                return;
            }

            var submitGuessUseCase = ProjectContext.Container.Build<SubmitGuessUseCase>();
            var command = new SubmitGuessCommand(submitGuessUseCase, gameState, guessWord);

            if (!command.CanExecute())
            {
                Debug.LogWarning("InputController: SubmitGuessCommand cannot execute");
                _isProcessingGuess = false;
                return;
            }

            try
            {
                await command.ExecuteAsync(_cts.Token);
                command.OnComplete();
            }
            catch (OperationCanceledException)
            {
                _isProcessingGuess = false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"InputController: Command execution failed - {ex}");
                command.OnFailed(ex);
                _isProcessingGuess = false;
            }
        }

        private void OnGuessSubmitted(GuessSubmittedEvent evt)
        {
            ClearCurrentWord();
            _currentRow++;
            _isProcessingGuess = false;
        }

        private void OnInvalidWordSubmitted(InvalidWordSubmittedEvent evt)
        {
            _isProcessingGuess = false;
        }

        private void OnGameStarted(GameStartedEvent evt)
        {
            ClearCurrentWord();
            _currentRow = 0;
            _isProcessingGuess = false;
        }

        private void ClearCurrentWord()
        {
            for (int i = 0; i < _currentWord.Length; i++)
            {
                _currentWord[i] = '\0';
            }
            _currentPosition = 0;
        }

        public void SetInputEnabled(bool enabled)
        {
            if (_keyboardInput != null)
            {
                _keyboardInput.IsEnabled = enabled;
            }

            if (_touchInput != null)
            {
                _touchInput.IsEnabled = enabled;
            }
        }

        public TouchInputService GetTouchInputService()
        {
            return _touchInput;
        }

        public void Dispose()
        {
            if (_keyboardInput != null)
            {
                _keyboardInput.OnLetterPressed -= HandleLetterPressed;
                _keyboardInput.OnBackspacePressed -= HandleBackspacePressed;
                _keyboardInput.OnEnterPressed -= HandleEnterPressed;
                _keyboardInput.Shutdown();
            }

            if (_touchInput != null)
            {
                _touchInput.OnLetterPressed -= HandleLetterPressed;
                _touchInput.OnBackspacePressed -= HandleBackspacePressed;
                _touchInput.OnEnterPressed -= HandleEnterPressed;
                _touchInput.Shutdown();
            }

            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<GuessSubmittedEvent>(OnGuessSubmitted);
                _eventBus.Unsubscribe<InvalidWordSubmittedEvent>(OnInvalidWordSubmitted);
                _eventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
            }

            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }
    }
}