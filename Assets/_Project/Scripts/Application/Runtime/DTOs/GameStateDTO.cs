using System;
using Wordle.Core.Entities;
using Wordle.Core.ValueObjects;

namespace Wordle.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for GameState.
    /// Used to transfer game state data between layers.
    /// </summary>
    public readonly struct GameStateDTO
    {
        public readonly string TargetWord;
        public readonly int MaxAttempts;
        public readonly int CurrentAttempt;
        public readonly int AttemptsRemaining;
        public readonly GameStatus Status;
        public readonly GuessDTO[] Guesses;
        public readonly DateTime StartTime;

        public GameStateDTO(
            string targetWord,
            int maxAttempts,
            int currentAttempt,
            int attemptsRemaining,
            GameStatus status,
            GuessDTO[] guesses,
            DateTime startTime)
        {
            TargetWord = targetWord;
            MaxAttempts = maxAttempts;
            CurrentAttempt = currentAttempt;
            AttemptsRemaining = attemptsRemaining;
            Status = status;
            Guesses = guesses;
            StartTime = startTime;
        }

        public static GameStateDTO FromEntity(GameState gameState)
        {
            if (gameState == null)
            {
                throw new ArgumentNullException(nameof(gameState));
            }

            var guesses = new GuessDTO[gameState.Guesses.Count];
            for (int i = 0; i < gameState.Guesses.Count; i++)
            {
                guesses[i] = GuessDTO.FromEntity(gameState.Guesses[i]);
            }

            return new GameStateDTO(
                gameState.TargetWord.Value,
                gameState.MaxAttempts,
                gameState.CurrentAttempt,
                gameState.AttemptsRemaining,
                gameState.Status,
                guesses,
                gameState.StartTime
            );
        }
    }

    /// <summary>
    /// Data Transfer Object for Guess.
    /// </summary>
    public readonly struct GuessDTO
    {
        public readonly string Word;
        public readonly LetterEvaluationDTO[] Evaluations;
        public readonly DateTime Timestamp;

        public GuessDTO(string word, LetterEvaluationDTO[] evaluations, DateTime timestamp)
        {
            Word = word;
            Evaluations = evaluations;
            Timestamp = timestamp;
        }

        public static GuessDTO FromEntity(Guess guess)
        {
            var evaluations = new LetterEvaluationDTO[guess.Evaluations.Length];
            for (int i = 0; i < guess.Evaluations.Length; i++)
            {
                evaluations[i] = LetterEvaluationDTO.FromValueObject(guess.Evaluations[i]);
            }

            return new GuessDTO(
                guess.GuessedWord.Value,
                evaluations,
                guess.Timestamp
            );
        }
    }

    /// <summary>
    /// Data Transfer Object for LetterPosition.
    /// </summary>
    public readonly struct LetterEvaluationDTO
    {
        public readonly char Letter;
        public readonly int Position;
        public readonly string Evaluation;

        public LetterEvaluationDTO(char letter, int position, string evaluation)
        {
            Letter = letter;
            Position = position;
            Evaluation = evaluation;
        }

        public static LetterEvaluationDTO FromValueObject(LetterPosition letterPosition)
        {
            return new LetterEvaluationDTO(
                letterPosition.Letter,
                letterPosition.Position,
                letterPosition.Evaluation.ToString()
            );
        }
    }
}
