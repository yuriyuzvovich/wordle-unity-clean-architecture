using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Wordle.Application.Interfaces;
using Wordle.Core.Interfaces;

namespace Wordle.Infrastructure.Persistence
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly ILogService _logService;
        private readonly IProjectConfigService _configService;
        private readonly ILocalStorageService _localStorageService;

        public StatisticsRepository(ILogService logService, IProjectConfigService configService, ILocalStorageService localStorageService)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            _localStorageService = localStorageService ?? throw new ArgumentNullException(nameof(localStorageService));
        }

        public UniTask SaveStatisticsAsync(int totalGamesPlayed, int totalGamesWon, int totalGamesLost, int currentStreak, int maxStreak, int[] winDistribution)
        {
            try
            {
                var data = new SerializableStatistics(totalGamesPlayed, totalGamesWon, totalGamesLost, currentStreak, maxStreak, winDistribution);
                var json = JsonUtility.ToJson(data);
                _localStorageService.SetString(_configService.StatisticsSaveKey, json);
                _localStorageService.Save();
                _logService.LogInfo("Statistics saved successfully");
                return UniTask.CompletedTask;
            }
            catch (Exception ex)
            {
                _logService.LogError($"Failed to save statistics: {ex.Message}");
                throw;
            }
        }

        public UniTask<(int totalGamesPlayed, int totalGamesWon, int totalGamesLost, int currentStreak, int maxStreak, int[] winDistribution)> LoadStatisticsAsync()
        {
            try
            {
                if (!_localStorageService.HasKey(_configService.StatisticsSaveKey))
                {
                    _logService.LogInfo("No saved statistics found, returning defaults");
                    return UniTask.FromResult((0, 0, 0, 0, 0, new int[6]));
                }

                var json = _localStorageService.GetString(_configService.StatisticsSaveKey);
                var data = JsonUtility.FromJson<SerializableStatistics>(json);
                _logService.LogInfo("Statistics loaded successfully");
                return UniTask.FromResult((data.totalGamesPlayed, data.totalGamesWon, data.totalGamesLost, data.currentStreak, data.maxStreak, data.winDistribution));
            }
            catch (Exception ex)
            {
                _logService.LogError($"Failed to load statistics: {ex.Message}");
                throw;
            }
        }

        public UniTask<bool> HasStatisticsAsync()
        {
            var hasStatistics = _localStorageService.HasKey(_configService.StatisticsSaveKey);
            return UniTask.FromResult(hasStatistics);
        }

        public UniTask ClearStatisticsAsync()
        {
            try
            {
                // Defensive: handle missing config service or empty key
                var key = _configService?.StatisticsSaveKey;
                if (string.IsNullOrEmpty(key))
                {
                    _logService?.LogWarning("StatisticsSaveKey is null or empty; nothing to clear.");
                    return UniTask.CompletedTask;
                }

                try
                {
                    if (_localStorageService.HasKey(key))
                    {
                        _localStorageService.DeleteKey(key);
                        _localStorageService.Save();
                        _logService?.LogInfo("Statistics cleared successfully");
                    }
                }
                catch (Exception ex)
                {
                    _logService?.LogWarning($"PlayerPrefs operation failed while clearing statistics: {ex.Message}");
                }

                return UniTask.CompletedTask;
            }
            catch (Exception ex)
            {
                _logService?.LogError($"Failed to clear statistics: {ex.Message}");
                throw;
            }
        }
    }
}
