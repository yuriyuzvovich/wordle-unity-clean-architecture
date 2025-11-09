namespace Wordle.Infrastructure.Configuration
{
    public static class ProjectConfigFactory
    {
        public static ProjectConfig CreateDefault()
        {
            return new ProjectConfig(
                wordLength: 5,
                maxAttempts: 6,
                targetWordsAddress: "TargetWords",
                validGuessWordsAddress: "ValidGuessWords",
                statisticsSaveKey: "Wordle_Statistics",
                gameStateSaveKey: "Wordle_GameState"
            );
        }

        public static ProjectConfig Create(
            int wordLength,
            int maxAttempts,
            string targetWordsAddress,
            string validGuessWordsAddress,
            string statisticsSaveKey,
            string gameStateSaveKey)
        {
            return new ProjectConfig(
                wordLength,
                maxAttempts,
                targetWordsAddress,
                validGuessWordsAddress,
                statisticsSaveKey,
                gameStateSaveKey
            );
        }
    }
}
