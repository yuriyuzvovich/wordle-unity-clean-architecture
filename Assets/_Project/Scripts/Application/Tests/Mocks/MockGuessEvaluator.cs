using Wordle.Core.Entities;
using Wordle.Core.Interfaces;
using Wordle.Core.ValueObjects;

namespace Wordle.Application.Tests.Mocks
{
    public class MockGuessEvaluator : IGuessEvaluator
    {
        public LetterPosition[] Evaluate(Word guessWord, Word targetWord)
        {
            var evaluations = new LetterPosition[Word.WORD_LENGTH];

            for (int i = 0; i < Word.WORD_LENGTH; i++)
            {
                char guessChar = guessWord[i];
                char targetChar = targetWord[i];

                LetterEvaluation evaluation;
                if (guessChar == targetChar)
                {
                    evaluation = LetterEvaluation.Correct;
                }
                else if (targetWord.Value.Contains(guessChar.ToString()))
                {
                    evaluation = LetterEvaluation.Present;
                }
                else
                {
                    evaluation = LetterEvaluation.Absent;
                }

                evaluations[i] = new LetterPosition(guessChar, i, evaluation);
            }

            return evaluations;
        }
    }
}
