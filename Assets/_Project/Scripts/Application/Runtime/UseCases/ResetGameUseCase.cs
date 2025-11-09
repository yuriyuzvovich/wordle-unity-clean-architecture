using System;
using Cysharp.Threading.Tasks;
using Wordle.Application.Interfaces;
using Wordle.Core.Entities;
using Wordle.Core.Events;
using Wordle.Core.Interfaces;

namespace Wordle.Application.UseCases
{
    /// <summary>
    /// Use case: Reset the current game and start a new one.
    /// Clears saved state, gets a new target word, and starts a fresh game.
    /// </summary>
    public class ResetGameUseCase
    {
        private readonly IEventBus _eventBus;
        private readonly IGameStateRepository _gameStateRepository;
        private readonly IWordRepository _wordRepository;

        public ResetGameUseCase(
            IEventBus eventBus,
            IGameStateRepository gameStateRepository,
            IWordRepository wordRepository)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _gameStateRepository = gameStateRepository ?? throw new ArgumentNullException(nameof(gameStateRepository));
            _wordRepository = wordRepository ?? throw new ArgumentNullException(nameof(wordRepository));
        }

        public async UniTask<ResetGameResult> ExecuteAsync()
        {
            await _gameStateRepository.ClearAsync();

            if (!_wordRepository.IsInitialized)
            {
                return new ResetGameResult(false, "Word repository is not initialized", null);
            }

            Word targetWord = await _wordRepository.GetRandomTargetWordAsync();

            var gameState = new GameState(targetWord);

            await _gameStateRepository.SaveAsync(gameState);

            _eventBus.Publish(new GameStartedEvent(
                targetWord.Value,
                gameState.MaxAttempts
            ));

            return new ResetGameResult(true, "Game reset successfully", gameState);
        }
    }

    public readonly struct ResetGameResult
    {
        public readonly bool Success;
        public readonly string Message;
        public readonly GameState GameState;

        public ResetGameResult(bool success, string message, GameState gameState)
        {
            Success = success;
            Message = message;
            GameState = gameState;
        }
    }
}
