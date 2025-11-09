using Wordle.Core.Interfaces;

namespace Wordle.Application.Tests.UseCases
{
    public class MockGameRules:IGameRules
    {
        public int WordLength { get; } = 5;
        public int MaxAttempts { get; } = 6;
        public bool IsWordLengthValid(int length)
        {
            return length == WordLength;
        }

        public bool IsAttemptCountValid(int attemptCount)
        {
            return attemptCount >= 0 && attemptCount <= MaxAttempts;
        }
    }
}