using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Wordle.Application.Interfaces;
using Wordle.Core.Entities;
using Wordle.Infrastructure.Persistence;

namespace Tests
{
    [TestFixture]
    public class GameStateRepositoryTests
    {
        private GameStateRepository _repository;
        private MockLogService _logService;
        private MockProjectConfigService _configService;
        private MockPlayerPrefsService _playerPrefsService;
        private const string SAVE_KEY = "Wordle_GameState";

        [SetUp]
        public void SetUp()
        {
            _logService = new MockLogService();
            _configService = new MockProjectConfigService();
            _playerPrefsService = new MockPlayerPrefsService();
            _repository = new GameStateRepository(_logService, _configService, _playerPrefsService);
        }

        #region Constructor Tests

        [Test]
        public void Constructor_WithLogService_CreatesInstance()
        {
            var repository = new GameStateRepository(_logService, _configService, _playerPrefsService);

            Assert.IsNotNull(repository);
        }

        [Test]
        public void Constructor_NullLogService_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GameStateRepository(null, _configService, _playerPrefsService));
        }

        [Test]
        public void Constructor_NullPlayerPrefsService_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GameStateRepository(_logService, _configService, null));
        }

        #endregion

        #region SaveAsync Tests

        [Test]
        public async Task SaveAsync_ValidGameState_SavesSuccessfully()
        {
            var targetWord = new Word("HELLO");
            var gameState = new GameState(targetWord, 6);

            await _repository.SaveAsync(gameState).AsTask();

            Assert.IsTrue(_playerPrefsService.HasKey(SAVE_KEY));
        }

        [Test]
        public async Task SaveAsync_SavesAndLoads_ReturnsSameData()
        {
            var targetWord = new Word("HELLO");
            var gameState = new GameState(targetWord, 6);

            await _repository.SaveAsync(gameState).AsTask();
            var loaded = await _repository.LoadAsync().AsTask();

            Assert.IsNotNull(loaded);
            Assert.AreEqual(gameState.TargetWord.Value, loaded.TargetWord.Value);
            Assert.AreEqual(gameState.MaxAttempts, loaded.MaxAttempts);
            Assert.AreEqual(gameState.Status, loaded.Status);
        }

        #endregion

        #region LoadAsync Tests

        [Test]
        public async Task LoadAsync_NoSavedState_ReturnsNull()
        {
            var loaded = await _repository.LoadAsync().AsTask();

            Assert.IsNull(loaded);
        }

        [Test]
        public async Task LoadAsync_AfterSave_ReturnsGameState()
        {
            var targetWord = new Word("HELLO");
            var gameState = new GameState(targetWord, 6);
            await _repository.SaveAsync(gameState).AsTask();

            var loaded = await _repository.LoadAsync().AsTask();

            Assert.IsNotNull(loaded);
        }

        #endregion

        #region HasSavedStateAsync Tests

        [Test]
        public async Task HasSavedStateAsync_NoSave_ReturnsFalse()
        {
            var hasSaved = await _repository.HasSavedStateAsync().AsTask();

            Assert.IsFalse(hasSaved);
        }

        [Test]
        public async Task HasSavedStateAsync_AfterSave_ReturnsTrue()
        {
            var targetWord = new Word("HELLO");
            var gameState = new GameState(targetWord, 6);
            await _repository.SaveAsync(gameState).AsTask();

            var hasSaved = await _repository.HasSavedStateAsync().AsTask();

            Assert.IsTrue(hasSaved);
        }

        #endregion

        #region ClearAsync Tests

        [Test]
        public async Task ClearAsync_RemovesSavedState()
        {
            var targetWord = new Word("HELLO");
            var gameState = new GameState(targetWord, 6);
            await _repository.SaveAsync(gameState).AsTask();

            await _repository.ClearAsync().AsTask();

            var hasSaved = await _repository.HasSavedStateAsync().AsTask();
            Assert.IsFalse(hasSaved);
        }

        [Test]
        public async Task ClearAsync_NoSavedState_DoesNotThrow()
        {
            await _repository.ClearAsync().AsTask();

            Assert.Pass();
        }

        #endregion

        #region Test Helper Classes

        private class MockLogService : ILogService
        {
            public void LogDebug(string message) { }
            public void LogInfo(string message) { }
            public void LogWarning(string message) { }
            public void LogError(string message) { }
            public void LogException(string message, Exception exception) { }
            public void SetLogLevel(LogLevel level) { }
        }

        private class MockProjectConfigService : IProjectConfigService
        {
            public int WordLength => 5;
            public int MaxAttempts => 6;
            public string TargetWordsAddress => "TargetWords";
            public string ValidGuessWordsAddress => "ValidGuessWords";
            public string StatisticsSaveKey => "Wordle_Statistics";
            public string GameStateSaveKey => "Wordle_GameState";
        }

        #endregion
    }
}
