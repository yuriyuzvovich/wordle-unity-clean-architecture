using System;
using Wordle.Application.Commands;
using Wordle.Application.Events;
using Wordle.Application.Interfaces;
using Wordle.Application.UseCases;
using Wordle.Infrastructure.Common.DI;
using Wordle.Core.Events;
using Wordle.Core.Interfaces;
using Cysharp.Threading.Tasks;

namespace Wordle.Presentation.Controllers
{
    /// <summary>
    /// Game controller demonstrating:
    /// - Pure C# class (no MonoBehaviour dependency)
    /// - Constructor injection
    /// - Command queue usage
    /// - Event bus subscription
    /// </summary>
    public class GameController : IDisposable
    {
        private readonly ICommandQueue _commandQueue;
        private readonly IEventBus _eventBus;
        private readonly ILogService _logger;
        private readonly IWordRepository _wordRepository;

        public GameController(
            ICommandQueue commandQueue,
            IEventBus eventBus,
            ILogService logger,
            IWordRepository wordRepository)
        {
            _commandQueue = commandQueue ?? throw new ArgumentNullException(nameof(commandQueue));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _wordRepository = wordRepository ?? throw new ArgumentNullException(nameof(wordRepository));
        }

        public void Initialize(bool autoStartGame = true, string testWord = "HELLO")
        {
            _logger.LogInfo("GameController: Dependencies injected successfully.");

            _eventBus.Subscribe<GameStartedEvent>(OnGameStarted);
            _eventBus.Subscribe<GuessSubmittedEvent>(OnGuessSubmitted);
            _eventBus.Subscribe<GameWonEvent>(OnGameWon);
            _eventBus.Subscribe<GameLostEvent>(OnGameLost);
            _eventBus.Subscribe<WordsReloadedEvent>(OnWordsReloaded);

            _logger.LogInfo("GameController: Subscribed to game events.");

            if (autoStartGame)
            {
                _logger.LogInfo("GameController: Auto-starting game...");
                StartNewGame(testWord);
            }
        }

        private void OnWordsReloaded(WordsReloadedEvent evt)
        {
            _logger.LogInfo($"GameController: Words reloaded for '{evt.LanguageCode}', restarting game...");
            RestartGameWithRandomWordAsync().Forget();
        }

        private async UniTaskVoid RestartGameWithRandomWordAsync()
        {
            try
            {
                var randomWord = await _wordRepository.GetRandomTargetWordAsync();
                StartNewGame(randomWord.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GameController: Failed to restart game with random word: {ex.Message}");
            }
        }

        public void StartNewGame(string targetWord)
        {
            _logger.LogInfo($"GameController: Queueing StartGame command with word '{targetWord}'");

            var startGameUseCase = ProjectContext.Container.Build<StartGameUseCase>();

            var command = new StartGameCommand(startGameUseCase, targetWord);
            _commandQueue.Enqueue(command);

            _logger.LogInfo($"GameController: Command queued. Total commands: {_commandQueue.TotalCommandsCount}");
        }

        private void OnGameStarted(GameStartedEvent evt)
        {
            _logger.LogInfo($"GameController received: {evt}");
        }

        private void OnGuessSubmitted(GuessSubmittedEvent evt)
        {
            _logger.LogInfo($"GameController received: {evt}");
        }

        private void OnGameWon(GameWonEvent evt)
        {
            _logger.LogInfo($"GameController received: {evt}");
        }

        private void OnGameLost(GameLostEvent evt)
        {
            _logger.LogInfo($"GameController received: {evt}");
        }

        public void Dispose()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<GameStartedEvent>(OnGameStarted);
                _eventBus.Unsubscribe<GuessSubmittedEvent>(OnGuessSubmitted);
                _eventBus.Unsubscribe<GameWonEvent>(OnGameWon);
                _eventBus.Unsubscribe<GameLostEvent>(OnGameLost);
                _eventBus.Unsubscribe<WordsReloadedEvent>(OnWordsReloaded);
            }
        }
    }
}
