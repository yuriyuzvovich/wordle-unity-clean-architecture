using Wordle.Core.Entities;

namespace Wordle.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for guess submission result.
    /// Contains the evaluation and game status after a guess.
    /// </summary>
    public readonly struct GuessResultDTO
    {
        public readonly bool Success;
        public readonly string Message;
        public readonly GuessDTO Guess;
        public readonly bool IsCorrect;
        public readonly GameStatus GameStatus;
        public readonly int AttemptsRemaining;

        public GuessResultDTO(
            bool success,
            string message,
            GuessDTO guess,
            bool isCorrect,
            GameStatus gameStatus,
            int attemptsRemaining)
        {
            Success = success;
            Message = message;
            Guess = guess;
            IsCorrect = isCorrect;
            GameStatus = gameStatus;
            AttemptsRemaining = attemptsRemaining;
        }

        public static GuessResultDTO CreateSuccess(
            GuessDTO guess,
            bool isCorrect,
            GameStatus gameStatus,
            int attemptsRemaining)
        {
            return new GuessResultDTO(
                true,
                "Guess submitted successfully",
                guess,
                isCorrect,
                gameStatus,
                attemptsRemaining
            );
        }

        public static GuessResultDTO CreateFailure(string message)
        {
            return new GuessResultDTO(
                false,
                message,
                default,
                false,
                GameStatus.Playing,
                0
            );
        }
    }
}
