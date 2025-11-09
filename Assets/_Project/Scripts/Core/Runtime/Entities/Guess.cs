using System;
using Wordle.Core.Interfaces;
using Wordle.Core.ValueObjects;

namespace Wordle.Core.Entities
{
    /// <summary>
    /// Represents a guess made by the player with evaluation results.
    /// Immutable entity.
    /// </summary>
    public readonly struct Guess
    {
        public readonly Word GuessedWord;
        public readonly LetterPosition[] Evaluations;
        public readonly DateTime Timestamp;

        public Guess(Word guessedWord, LetterPosition[] evaluations)
        {
            if (evaluations == null)
            {
                throw new ArgumentNullException(nameof(evaluations));
            }

            // Validate evaluations length matches the guessed word length
            if (evaluations.Length != guessedWord.Length)
            {
                throw new ArgumentException($"Evaluations length must match the guessed word length ({guessedWord.Length}).", nameof(evaluations));
            }

            GuessedWord = guessedWord;
            Evaluations = evaluations;
            Timestamp = DateTime.UtcNow;
        }

        public bool IsCorrect()
        {
            foreach (var evaluation in Evaluations)
            {
                if (evaluation.Evaluation != LetterEvaluation.Correct)
                {
                    return false;
                }
            }
            return true;
        }

        public override string ToString()
        {
            return $"{GuessedWord} at {Timestamp:HH:mm:ss}";
        }
    }
}
