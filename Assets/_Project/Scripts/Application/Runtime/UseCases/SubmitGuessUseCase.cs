using System;
using Cysharp.Threading.Tasks;
using Wordle.Application.DTOs;
using Wordle.Application.Interfaces;
using Wordle.Core.Entities;
using Wordle.Core.Events;
using Wordle.Core.Interfaces;

namespace Wordle.Application.UseCases
{
    /// <summary>
    /// Use case: Submit a guess and evaluate it against the target word.
    /// Handles word validation, guess evaluation, state updates, and event publishing.
    /// </summary>
    public class SubmitGuessUseCase
    {
        private readonly IEventBus _eventBus;
        private readonly IWordValidator _wordValidator;
        private readonly IGuessEvaluator _guessEvaluator;
        private readonly IGameStateRepository _gameStateRepository;
        private readonly IGameRules _gameRules;

        public SubmitGuessUseCase(
            IEventBus eventBus,
            IWordValidator wordValidator,
            IGuessEvaluator guessEvaluator,
            IGameStateRepository gameStateRepository,
            IGameRules gameRules
        )
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _wordValidator = wordValidator ?? throw new ArgumentNullException(nameof(wordValidator));
            _guessEvaluator = guessEvaluator ?? throw new ArgumentNullException(nameof(guessEvaluator));
            _gameStateRepository = gameStateRepository ?? throw new ArgumentNullException(nameof(gameStateRepository));
            _gameRules = gameRules ?? throw new ArgumentNullException(nameof(gameRules));
        }

        public async UniTask<GuessResultDTO> ExecuteAsync(GameState gameState, string guessWord)
        {
            if (gameState == null)
            {
                return GuessResultDTO.CreateFailure("Game state is null");
            }

            if (string.IsNullOrEmpty(guessWord))
            {
                return GuessResultDTO.CreateFailure("Guess word cannot be empty");
            }

            if (!_gameRules.IsWordLengthValid(guessWord.Length))
            {
                return GuessResultDTO.CreateFailure("Evaluated word length is invalid");
            }

            if (!gameState.CanMakeGuess())
            {
                return GuessResultDTO.CreateFailure("Cannot make a guess. Game is over or no attempts remaining.");
            }

            if (!_wordValidator.IsValidWord(guessWord))
            {
                string reason = "Word is not in the word list";
                _eventBus.Publish(new InvalidWordSubmittedEvent(guessWord, reason));
                return GuessResultDTO.CreateFailure(reason);
            }

            Word guessWordEntity;
            try
            {
                guessWordEntity = new Word(guessWord);
            }
            catch (ArgumentException ex)
            {
                return GuessResultDTO.CreateFailure(ex.Message);
            }

            var evaluations = _guessEvaluator.Evaluate(guessWordEntity, gameState.TargetWord);

            var guess = new Guess(guessWordEntity, evaluations);

            gameState.AddGuess(guess);

            var guessDto = GuessDTO.FromEntity(guess);
            bool isCorrect = guess.IsCorrect();

            await _gameStateRepository.SaveAsync(gameState);

            _eventBus.Publish(
                new GuessSubmittedEvent(
                    guessWord,
                    gameState.AttemptsRemaining,
                    isCorrect
                )
            );

            if (gameState.Status == GameStatus.Won)
            {
                _eventBus.Publish(
                    new GameWonEvent(
                        gameState.TargetWord.Value,
                        gameState.CurrentAttempt
                    )
                );
            }
            else if (gameState.Status == GameStatus.Lost)
            {
                _eventBus.Publish(
                    new GameLostEvent(
                        gameState.TargetWord.Value,
                        gameState.CurrentAttempt
                    )
                );
            }

            return GuessResultDTO.CreateSuccess(
                guessDto,
                isCorrect,
                gameState.Status,
                gameState.AttemptsRemaining
            );
        }
    }
}