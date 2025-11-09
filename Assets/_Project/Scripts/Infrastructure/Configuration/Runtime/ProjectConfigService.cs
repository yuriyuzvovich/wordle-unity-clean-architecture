using System;
using Wordle.Application.Interfaces;

namespace Wordle.Infrastructure.Configuration
{
    public class ProjectConfigService : IProjectConfigService
    {
        private readonly ILogService _logService;
        private readonly ProjectConfig _config;

        public int WordLength => _config.WordLength;
        public int MaxAttempts => _config.MaxAttempts;
        public string TargetWordsAddress => _config.TargetWordsAddress;
        public string ValidGuessWordsAddress => _config.ValidGuessWordsAddress;
        public string StatisticsSaveKey => _config.StatisticsSaveKey;
        public string GameStateSaveKey => _config.GameStateSaveKey;

        public ProjectConfigService(ProjectConfig config, ILogService logService)
        {
            _config = config;
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));

            _logService.LogInfo("ProjectConfigService initialized successfully");
        }
    }
}
