using Wordle.Application.Interfaces;

namespace Wordle.Application.Tests.Mocks
{
    public class MockProjectConfigService : IProjectConfigService
    {
        public int WordLength => 5;
        public int MaxAttempts => 6;
        public string TargetWordsAddress => string.Empty;
        public string ValidGuessWordsAddress => string.Empty;
        public string StatisticsSaveKey => string.Empty;
        public string GameStateSaveKey => string.Empty;
    }
}

