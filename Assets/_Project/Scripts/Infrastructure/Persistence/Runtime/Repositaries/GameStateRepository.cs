using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Wordle.Application.Interfaces;
using Wordle.Core.Entities;
using Wordle.Core.Interfaces;

namespace Wordle.Infrastructure.Persistence
{
    public class GameStateRepository : IGameStateRepository
    {
        private readonly ILogService _logService;
        private readonly IProjectConfigService _configService;
        private readonly ILocalStorageService _localStorageService;

        public GameStateRepository(ILogService logService, IProjectConfigService configService, ILocalStorageService localStorageService)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            _localStorageService = localStorageService ?? throw new ArgumentNullException(nameof(localStorageService));
        }

        public UniTask SaveAsync(GameState gameState)
        {
            try
            {
                var data = SerializableGameState.FromGameState(gameState);
                var json = JsonUtility.ToJson(data);
                _localStorageService.SetString(_configService.GameStateSaveKey, json);
                _localStorageService.Save();
                _logService.LogInfo("Game state saved successfully");
                return UniTask.CompletedTask;
            }
            catch (Exception ex)
            {
                _logService.LogError($"Failed to save game state: {ex.Message}");
                throw;
            }
        }

        public UniTask<GameState> LoadAsync()
        {
            try
            {
                if (!_localStorageService.HasKey(_configService.GameStateSaveKey))
                {
                    _logService.LogInfo("No saved game state found");
                    return UniTask.FromResult<GameState>(null);
                }

                var json = _localStorageService.GetString(_configService.GameStateSaveKey);
                var data = JsonUtility.FromJson<SerializableGameState>(json);
                var gameState = data.ToGameState();
                _logService.LogInfo("Game state loaded successfully");
                return UniTask.FromResult(gameState);
            }
            catch (Exception ex)
            {
                _logService.LogError($"Failed to load game state: {ex.Message}");
                throw;
            }
        }

        public UniTask<bool> HasSavedStateAsync()
        {
            var hasSavedState = _localStorageService.HasKey(_configService.GameStateSaveKey);
            return UniTask.FromResult(hasSavedState);
        }

        public UniTask ClearAsync()
        {
            try
            {
                // Defensive: handle missing config service or empty key
                var key = _configService?.GameStateSaveKey;
                if (string.IsNullOrEmpty(key))
                {
                    _logService?.LogWarning("GameStateSaveKey is null or empty; nothing to clear.");
                    return UniTask.CompletedTask;
                }

                try
                {
                    if (_localStorageService.HasKey(key))
                    {
                        _localStorageService.DeleteKey(key);
                        _localStorageService.Save();
                        _logService?.LogInfo("Game state cleared successfully");
                    }
                }
                catch (Exception ex)
                {
                    _logService?.LogWarning($"PlayerPrefs operation failed while clearing game state: {ex.Message}");
                }

                return UniTask.CompletedTask;
            }
            catch (Exception ex)
            {
                _logService?.LogError($"Failed to clear game state: {ex.Message}");
                throw;
            }
        }
    }
}
