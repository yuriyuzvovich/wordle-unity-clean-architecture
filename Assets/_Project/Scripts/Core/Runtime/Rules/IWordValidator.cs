using Wordle.Core.Entities;

namespace Wordle.Core.Interfaces
{
    /// <summary>
    /// Domain service interface: Validates if a word is in the valid word list.
    /// </summary>
    public interface IWordValidator
    {
        bool IsValidWord(Word word);
        bool IsValidWord(string word);
    }
}
