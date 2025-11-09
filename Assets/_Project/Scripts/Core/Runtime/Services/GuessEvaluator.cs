using System.Collections.Generic;
using Wordle.Core.Entities;
using Wordle.Core.Interfaces;
using Wordle.Core.ValueObjects;

namespace Wordle.Core.Services
{
    /// <summary>
    /// Domain service: Evaluates guesses against target words using Wordle rules.
    /// Handles duplicate letters correctly.
    /// </summary>
    public class GuessEvaluator : IGuessEvaluator
    {
        private readonly IGameRules _gameRules;

        public GuessEvaluator(IGameRules gameRules)
        {
            _gameRules = gameRules ?? throw new System.ArgumentNullException(nameof(gameRules));
        }

        public LetterPosition[] Evaluate(Word guessedWord, Word targetWord)
        {
            var evaluations = new LetterPosition[_gameRules.WordLength];
            var targetLetterCounts = new Dictionary<char, int>();

            for (int i = 0; i < _gameRules.WordLength; i++)
            {
                char letter = targetWord[i];
                if (!targetLetterCounts.ContainsKey(letter))
                {
                    targetLetterCounts[letter] = 0;
                }
                targetLetterCounts[letter]++;
            }

            for (int i = 0; i < _gameRules.WordLength; i++)
            {
                char guessedLetter = guessedWord[i];
                char targetLetter = targetWord[i];

                if (guessedLetter == targetLetter)
                {
                    evaluations[i] = new LetterPosition(guessedLetter, i, LetterEvaluation.Correct);
                    targetLetterCounts[guessedLetter]--;
                }
                else
                {
                    evaluations[i] = new LetterPosition(guessedLetter, i, LetterEvaluation.Absent);
                }
            }

            for (int i = 0; i < _gameRules.WordLength; i++)
            {
                if (evaluations[i].Evaluation == LetterEvaluation.Correct)
                {
                    continue;
                }

                char guessedLetter = guessedWord[i];

                if (targetLetterCounts.ContainsKey(guessedLetter) && targetLetterCounts[guessedLetter] > 0)
                {
                    evaluations[i] = new LetterPosition(guessedLetter, i, LetterEvaluation.Present);
                    targetLetterCounts[guessedLetter]--;
                }
            }

            return evaluations;
        }
    }
}