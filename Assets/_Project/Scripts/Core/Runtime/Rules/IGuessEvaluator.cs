using Wordle.Core.Entities;
using Wordle.Core.ValueObjects;

namespace Wordle.Core.Interfaces
{
    /// <summary>
    /// Domain service interface: Evaluates a guess against the target word.
    /// </summary>
    public interface IGuessEvaluator
    {
        LetterPosition[] Evaluate(Word guessedWord, Word targetWord);
    }
}
