using System;
using System.Collections.Generic;

namespace Wordle.Core.Entities
{
    /// <summary>
    /// Represents the current state of a Wordle game.
    /// </summary>
    public class GameState
    {
        public const int DEFAULT_MAX_ATTEMPTS = 6;

        public readonly Word TargetWord;
        public readonly int MaxAttempts;
        public readonly DateTime StartTime;

        private readonly List<Guess> _guesses;

        public IReadOnlyList<Guess> Guesses => _guesses;
        public int AttemptsRemaining => MaxAttempts - _guesses.Count;
        public int CurrentAttempt => _guesses.Count;
        public GameStatus Status { get; private set; }

        public GameState(Word targetWord, int maxAttempts = DEFAULT_MAX_ATTEMPTS)
        {
            if (maxAttempts <= 0)
            {
                throw new ArgumentException("Max attempts must be greater than 0", nameof(maxAttempts));
            }

            TargetWord = targetWord;
            MaxAttempts = maxAttempts;
            StartTime = DateTime.UtcNow;
            Status = GameStatus.Playing;
            _guesses = new List<Guess>(maxAttempts);
        }

        public bool CanMakeGuess()
        {
            return Status == GameStatus.Playing && AttemptsRemaining > 0;
        }

        public void AddGuess(Guess guess)
        {
            if (!CanMakeGuess())
            {
                throw new InvalidOperationException("Cannot make a guess. Game is over or no attempts remaining.");
            }

            _guesses.Add(guess);

            if (guess.IsCorrect())
            {
                Status = GameStatus.Won;
            }
            else if (AttemptsRemaining == 0)
            {
                Status = GameStatus.Lost;
            }
        }

        public Guess GetLastGuess()
        {
            if (_guesses.Count == 0)
            {
                throw new InvalidOperationException("No guesses have been made yet.");
            }

            return _guesses[_guesses.Count - 1];
        }

        public bool HasGuesses()
        {
            return _guesses.Count > 0;
        }
    }

    public enum GameStatus
    {
        Playing = 0,
        Won = 1,
        Lost = 2
    }
}
