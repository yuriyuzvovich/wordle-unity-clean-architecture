namespace Wordle.Infrastructure.Configuration
{
    public class ProjectConfig
    {
        public readonly int WordLength;
        public readonly int MaxAttempts;
        public readonly string TargetWordsAddress;
        public readonly string ValidGuessWordsAddress;
        public readonly string StatisticsSaveKey;
        public readonly string GameStateSaveKey;

        public ProjectConfig(
            int wordLength,
            int maxAttempts,
            string targetWordsAddress,
            string validGuessWordsAddress,
            string statisticsSaveKey,
            string gameStateSaveKey)
        {
            WordLength = wordLength;
            MaxAttempts = maxAttempts;
            TargetWordsAddress = targetWordsAddress;
            ValidGuessWordsAddress = validGuessWordsAddress;
            StatisticsSaveKey = statisticsSaveKey;
            GameStateSaveKey = gameStateSaveKey;
        }
    }
}