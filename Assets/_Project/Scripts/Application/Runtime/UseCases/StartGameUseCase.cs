using System;
using Cysharp.Threading.Tasks;
using Wordle.Application.Interfaces;
using Wordle.Core.Entities;
using Wordle.Core.Events;
using Wordle.Core.Interfaces;

namespace Wordle.Application.UseCases
{
    /// <summary>
    /// Use case: Start a new Wordle game.
    /// Demonstrates dependency injection and event publishing.
    /// </summary>
    public class StartGameUseCase
    {
        private readonly IEventBus _eventBus;
        private readonly IProjectConfigService _configService;
        private readonly IGameStateRepository _gameStateRepository;

        public StartGameUseCase(
            IEventBus eventBus,
            IProjectConfigService configService,
            IGameStateRepository gameStateRepository)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            _gameStateRepository = gameStateRepository ?? throw new ArgumentNullException(nameof(gameStateRepository));
        }

        public async UniTask<GameStartResult> ExecuteAsync(string targetWord)
        {
            // Validate input
            if (string.IsNullOrEmpty(targetWord))
            {
                return new GameStartResult(false, "Target word cannot be empty");
            }

            if (targetWord.Length != _configService.WordLength)
            {
                return new GameStartResult(false, $"Target word must be {_configService.WordLength} letters");
            }

            // Create target word entity
            Word targetWordEntity;
            try
            {
                targetWordEntity = new Word(targetWord);
            }
            catch (ArgumentException ex)
            {
                return new GameStartResult(false, $"Invalid target word: {ex.Message}");
            }

            // Create new game state
            var gameState = new GameState(targetWordEntity, _configService.MaxAttempts);

            // Save game state
            await _gameStateRepository.SaveAsync(gameState);

            // Publish domain event
            _eventBus.Publish(new GameStartedEvent(targetWord, _configService.MaxAttempts));

            return new GameStartResult(true, "Game started successfully");
        }
    }

    public struct GameStartResult
    {
        public readonly bool Success;
        public readonly string Message;

        public GameStartResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
