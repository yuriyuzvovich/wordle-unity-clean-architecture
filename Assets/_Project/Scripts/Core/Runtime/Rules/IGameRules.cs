namespace Wordle.Core.Interfaces
{
    /// <summary>
    /// Domain service interface: Encapsulates Wordle game rules.
    /// </summary>
    public interface IGameRules
    {
        int WordLength { get; }
        int MaxAttempts { get; }

        bool IsWordLengthValid(int length);
        bool IsAttemptCountValid(int attemptCount);
    }
}
