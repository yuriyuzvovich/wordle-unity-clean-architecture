using Wordle.Core.Entities;
using Wordle.Core.Interfaces;

namespace Wordle.Core.Services
{
    /// <summary>
    /// Domain service: Implements standard Wordle game rules.
    /// </summary>
    public class GameRules : IGameRules
    {
        public int WordLength => Word.WORD_LENGTH;
        public int MaxAttempts => GameState.DEFAULT_MAX_ATTEMPTS;

        public bool IsWordLengthValid(int length)
        {
            return length == WordLength;
        }

        public bool IsAttemptCountValid(int attemptCount)
        {
            return attemptCount > 0 && attemptCount <= MaxAttempts;
        }
    }
}
