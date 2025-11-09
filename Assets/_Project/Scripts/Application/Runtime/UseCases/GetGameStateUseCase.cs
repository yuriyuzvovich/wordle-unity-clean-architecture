using System;
using Cysharp.Threading.Tasks;
using Wordle.Application.DTOs;
using Wordle.Core.Interfaces;

namespace Wordle.Application.UseCases
{
    /// <summary>
    /// Use case: Get the current game state.
    /// Loads the saved game state from the repository.
    /// </summary>
    public class GetGameStateUseCase
    {
        private readonly IGameStateRepository _gameStateRepository;

        public GetGameStateUseCase(IGameStateRepository gameStateRepository)
        {
            _gameStateRepository = gameStateRepository ?? throw new ArgumentNullException(nameof(gameStateRepository));
        }

        public async UniTask<GameStateResult> ExecuteAsync()
        {
            bool hasSavedState = await _gameStateRepository.HasSavedStateAsync();

            if (!hasSavedState)
            {
                return new GameStateResult(false, "No saved game state found", null);
            }

            var gameState = await _gameStateRepository.LoadAsync();

            if (gameState == null)
            {
                return new GameStateResult(false, "Failed to load game state", null);
            }

            var gameStateDto = GameStateDTO.FromEntity(gameState);

            return new GameStateResult(true, "Game state loaded successfully", gameStateDto);
        }
    }

    public readonly struct GameStateResult
    {
        public readonly bool Success;
        public readonly string Message;
        public readonly GameStateDTO? GameState;

        public GameStateResult(bool success, string message, GameStateDTO? gameState)
        {
            Success = success;
            Message = message;
            GameState = gameState;
        }
    }
}
