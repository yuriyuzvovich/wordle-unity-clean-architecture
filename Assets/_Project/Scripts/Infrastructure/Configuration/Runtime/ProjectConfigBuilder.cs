namespace Wordle.Infrastructure.Configuration
{
    public class ProjectConfigBuilder
    {
        private int _wordLength = 5;
        private int _maxAttempts = 6;
        private string _targetWordsAddress = "TargetWords";
        private string _validGuessWordsAddress = "ValidGuessWords";
        private string _statisticsSaveKey = "Wordle_Statistics";
        private string _gameStateSaveKey = "Wordle_GameState";

        public ProjectConfigBuilder WithWordLength(int wordLength)
        {
            _wordLength = wordLength;
            return this;
        }

        public ProjectConfigBuilder WithMaxAttempts(int maxAttempts)
        {
            _maxAttempts = maxAttempts;
            return this;
        }

        public ProjectConfigBuilder WithTargetWordsAddress(string targetWordsAddress)
        {
            _targetWordsAddress = targetWordsAddress;
            return this;
        }

        public ProjectConfigBuilder WithValidGuessWordsAddress(string validGuessWordsAddress)
        {
            _validGuessWordsAddress = validGuessWordsAddress;
            return this;
        }

        public ProjectConfigBuilder WithStatisticsSaveKey(string statisticsSaveKey)
        {
            _statisticsSaveKey = statisticsSaveKey;
            return this;
        }

        public ProjectConfigBuilder WithGameStateSaveKey(string gameStateSaveKey)
        {
            _gameStateSaveKey = gameStateSaveKey;
            return this;
        }

        public ProjectConfig Build()
        {
            return new ProjectConfig(
                _wordLength,
                _maxAttempts,
                _targetWordsAddress,
                _validGuessWordsAddress,
                _statisticsSaveKey,
                _gameStateSaveKey
            );
        }
    }
}
