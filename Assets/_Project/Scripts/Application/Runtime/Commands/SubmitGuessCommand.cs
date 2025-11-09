using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Wordle.Application.DTOs;
using Wordle.Application.Interfaces;
using Wordle.Application.UseCases;
using Wordle.Core.Entities;

namespace Wordle.Application.Commands
{
    /// <summary>
    /// Command: Submit a guess to the game.
    /// Executes the SubmitGuessUseCase with the provided word.
    /// </summary>
    public class SubmitGuessCommand : ICommand
    {
        private readonly SubmitGuessUseCase _useCase;
        private readonly GameState _gameState;
        private readonly string _guessWord;

        public string CommandName => "SubmitGuess";
        public CommandPriority Priority => CommandPriority.High;

        public SubmitGuessCommand(
            SubmitGuessUseCase useCase,
            GameState gameState,
            string guessWord)
        {
            _useCase = useCase ?? throw new ArgumentNullException(nameof(useCase));
            _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
            _guessWord = guessWord;
        }

        public bool CanExecute()
        {
            return !string.IsNullOrEmpty(_guessWord) && _gameState.CanMakeGuess();
        }

        public async UniTask ExecuteAsync(CancellationToken ct)
        {
            GuessResultDTO result = await _useCase.ExecuteAsync(_gameState, _guessWord);
            // If the use case indicates failure (e.g., invalid word), surface it as an exception
            // so callers (and tests) can react to the failure appropriately.
            if (!result.Success)
            {
                throw new InvalidOperationException(result.Message ?? "Guess submission failed");
            }
        }

        public void OnComplete()
        {
        }

        public void OnFailed(Exception ex)
        {
        }
    }
}
