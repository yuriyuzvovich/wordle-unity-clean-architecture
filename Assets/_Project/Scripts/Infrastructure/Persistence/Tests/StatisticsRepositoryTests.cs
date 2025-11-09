using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Wordle.Application.Interfaces;
using Wordle.Infrastructure.Persistence;

namespace Tests
{
    [TestFixture]
    public class StatisticsRepositoryTests
    {
        private StatisticsRepository _repository;
        private MockLogService _logService;
        private MockProjectConfigService _configService;
        private MockPlayerPrefsService _playerPrefsService;
        private const string SAVE_KEY = "Wordle_Statistics";

        [SetUp]
        public void SetUp()
        {
            _logService = new MockLogService();
            _configService = new MockProjectConfigService();
            _playerPrefsService = new MockPlayerPrefsService();
            _repository = new StatisticsRepository(_logService, _configService, _playerPrefsService);
        }

        #region Constructor Tests

        [Test]
        public void Constructor_WithLogService_CreatesInstance()
        {
            var repository = new StatisticsRepository(_logService, _configService, _playerPrefsService);

            Assert.IsNotNull(repository);
        }

        [Test]
        public void Constructor_NullLogService_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new StatisticsRepository(null, _configService, _playerPrefsService));
        }

        [Test]
        public void Constructor_NullPlayerPrefsService_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new StatisticsRepository(_logService, _configService, null));
        }

        #endregion

        #region SaveStatisticsAsync Tests

        [Test]
        public async Task SaveStatisticsAsync_ValidData_SavesSuccessfully()
        {
            var winDistribution = new int[] { 0, 1, 2, 3, 4, 5 };

            await _repository.SaveStatisticsAsync(10, 8, 2, 3, 5, winDistribution).AsTask();

            Assert.IsTrue(_playerPrefsService.HasKey(SAVE_KEY));
        }

        [Test]
        public async Task SaveStatisticsAsync_SavesAndLoads_ReturnsSameData()
        {
            var winDistribution = new int[] { 1, 2, 3, 4, 5, 6 };

            await _repository.SaveStatisticsAsync(15, 12, 3, 5, 7, winDistribution).AsTask();
            var loaded = await _repository.LoadStatisticsAsync().AsTask();

            Assert.AreEqual(15, loaded.totalGamesPlayed);
            Assert.AreEqual(12, loaded.totalGamesWon);
            Assert.AreEqual(3, loaded.totalGamesLost);
            Assert.AreEqual(5, loaded.currentStreak);
            Assert.AreEqual(7, loaded.maxStreak);
            Assert.AreEqual(6, loaded.winDistribution.Length);
            CollectionAssert.AreEqual(winDistribution, loaded.winDistribution);
        }

        #endregion

        #region LoadStatisticsAsync Tests

        [Test]
        public async Task LoadStatisticsAsync_NoSavedStatistics_ReturnsDefaults()
        {
            var loaded = await _repository.LoadStatisticsAsync().AsTask();

            Assert.AreEqual(0, loaded.totalGamesPlayed);
            Assert.AreEqual(0, loaded.totalGamesWon);
            Assert.AreEqual(0, loaded.totalGamesLost);
            Assert.AreEqual(0, loaded.currentStreak);
            Assert.AreEqual(0, loaded.maxStreak);
            Assert.AreEqual(6, loaded.winDistribution.Length);
            Assert.AreEqual(0, loaded.winDistribution[0]);
        }

        [Test]
        public async Task LoadStatisticsAsync_AfterSave_ReturnsStatistics()
        {
            var winDistribution = new int[] { 10, 20, 30, 40, 50, 60 };
            await _repository.SaveStatisticsAsync(100, 85, 15, 10, 25, winDistribution).AsTask();

            var loaded = await _repository.LoadStatisticsAsync().AsTask();

            Assert.AreEqual(100, loaded.totalGamesPlayed);
            Assert.AreEqual(85, loaded.totalGamesWon);
            Assert.AreEqual(15, loaded.totalGamesLost);
            Assert.AreEqual(10, loaded.currentStreak);
            Assert.AreEqual(25, loaded.maxStreak);
            CollectionAssert.AreEqual(winDistribution, loaded.winDistribution);
        }

        #endregion

        #region HasStatisticsAsync Tests

        [Test]
        public async Task HasStatisticsAsync_NoSave_ReturnsFalse()
        {
            var hasStatistics = await _repository.HasStatisticsAsync().AsTask();

            Assert.IsFalse(hasStatistics);
        }

        [Test]
        public async Task HasStatisticsAsync_AfterSave_ReturnsTrue()
        {
            var winDistribution = new int[] { 1, 2, 3, 4, 5, 6 };
            await _repository.SaveStatisticsAsync(10, 8, 2, 3, 5, winDistribution).AsTask();

            var hasStatistics = await _repository.HasStatisticsAsync().AsTask();

            Assert.IsTrue(hasStatistics);
        }

        #endregion

        #region ClearStatisticsAsync Tests

        [Test]
        public async Task ClearStatisticsAsync_RemovesSavedStatistics()
        {
            var winDistribution = new int[] { 1, 2, 3, 4, 5, 6 };
            await _repository.SaveStatisticsAsync(10, 8, 2, 3, 5, winDistribution).AsTask();

            await _repository.ClearStatisticsAsync().AsTask();

            var hasStatistics = await _repository.HasStatisticsAsync().AsTask();
            Assert.IsFalse(hasStatistics);
        }

        [Test]
        public async Task ClearStatisticsAsync_NoSavedStatistics_DoesNotThrow()
        {
            await _repository.ClearStatisticsAsync().AsTask();

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
            public string GameStateSaveKey => "GameState";
        }

        #endregion
    }
}
