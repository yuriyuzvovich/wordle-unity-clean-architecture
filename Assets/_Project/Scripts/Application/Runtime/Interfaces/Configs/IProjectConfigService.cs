namespace Wordle.Application.Interfaces
{
    public interface IProjectConfigService
    {
        int WordLength { get; }
        int MaxAttempts { get; }
        string TargetWordsAddress { get; }
        string ValidGuessWordsAddress { get; }
        string StatisticsSaveKey { get; }
        string GameStateSaveKey { get; }
    }
}
