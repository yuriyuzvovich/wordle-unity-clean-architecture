using System;
using NUnit.Framework;
using Wordle.Application.Interfaces;
using Wordle.Infrastructure.Configuration;

namespace Tests
{
    [TestFixture]
    public class ProjectConfigServiceTests
    {
        private MockLogService _logService;

        [SetUp]
        public void SetUp()
        {
            _logService = new MockLogService();
        }

        [Test]
        public void Constructor_WithValidConfig_InitializesService()
        {
            var config = ProjectConfigFactory.CreateDefault();
            var configService = new ProjectConfigService(config, _logService);

            Assert.AreEqual(5, configService.WordLength);
            Assert.AreEqual(6, configService.MaxAttempts);
            Assert.AreEqual("TargetWords", configService.TargetWordsAddress);
            Assert.AreEqual("ValidGuessWords", configService.ValidGuessWordsAddress);
            Assert.AreEqual("Wordle_Statistics", configService.StatisticsSaveKey);
            Assert.AreEqual("Wordle_GameState", configService.GameStateSaveKey);
        }

        [Test]
        public void Constructor_WithCustomConfig_InitializesWithCustomValues()
        {
            var config = new ProjectConfig(
                wordLength : 6,
                maxAttempts : 8,
                targetWordsAddress : "CustomTarget",
                validGuessWordsAddress : "CustomGuess",
                statisticsSaveKey : "CustomStats",
                gameStateSaveKey : "CustomGameState"
            );
            var configService = new ProjectConfigService(config, _logService);

            Assert.AreEqual(6, configService.WordLength);
            Assert.AreEqual(8, configService.MaxAttempts);
            Assert.AreEqual("CustomTarget", configService.TargetWordsAddress);
            Assert.AreEqual("CustomGuess", configService.ValidGuessWordsAddress);
            Assert.AreEqual("CustomStats", configService.StatisticsSaveKey);
            Assert.AreEqual("CustomGameState", configService.GameStateSaveKey);
        }

        [Test]
        public void Constructor_NullLogService_ThrowsArgumentNullException()
        {
            var config = ProjectConfigFactory.CreateDefault();
            Assert.Throws<ArgumentNullException>(() => new ProjectConfigService(config, null));
        }
    }

    [TestFixture]
    public class ProjectConfigFactoryTests
    {
        [Test]
        public void CreateDefault_ReturnsDefaultConfiguration()
        {
            var config = ProjectConfigFactory.CreateDefault();

            Assert.AreEqual(5, config.WordLength);
            Assert.AreEqual(6, config.MaxAttempts);
            Assert.AreEqual("TargetWords", config.TargetWordsAddress);
            Assert.AreEqual("ValidGuessWords", config.ValidGuessWordsAddress);
            Assert.AreEqual("Wordle_Statistics", config.StatisticsSaveKey);
            Assert.AreEqual("Wordle_GameState", config.GameStateSaveKey);
        }

        [Test]
        public void Create_WithCustomValues_ReturnsCustomConfiguration()
        {
            var config = ProjectConfigFactory.Create(
                7,
                10,
                "Test1",
                "Test2",
                "Test3",
                "Test4"
            );

            Assert.AreEqual(7, config.WordLength);
            Assert.AreEqual(10, config.MaxAttempts);
            Assert.AreEqual("Test1", config.TargetWordsAddress);
            Assert.AreEqual("Test2", config.ValidGuessWordsAddress);
            Assert.AreEqual("Test3", config.StatisticsSaveKey);
            Assert.AreEqual("Test4", config.GameStateSaveKey);
        }
    }

    [TestFixture]
    public class ProjectConfigBuilderTests
    {
        [Test]
        public void Build_WithDefaults_ReturnsDefaultConfiguration()
        {
            var config = new ProjectConfigBuilder().Build();

            Assert.AreEqual(5, config.WordLength);
            Assert.AreEqual(6, config.MaxAttempts);
            Assert.AreEqual("TargetWords", config.TargetWordsAddress);
            Assert.AreEqual("ValidGuessWords", config.ValidGuessWordsAddress);
            Assert.AreEqual("Wordle_Statistics", config.StatisticsSaveKey);
            Assert.AreEqual("Wordle_GameState", config.GameStateSaveKey);
        }

        [Test]
        public void Build_WithCustomWordLength_OverridesDefault()
        {
            var config = new ProjectConfigBuilder()
                .WithWordLength(7)
                .Build();

            Assert.AreEqual(7, config.WordLength);
            Assert.AreEqual(6, config.MaxAttempts);
        }

        [Test]
        public void Build_WithMultipleCustomValues_OverridesMultipleDefaults()
        {
            var config = new ProjectConfigBuilder()
                .WithWordLength(8)
                .WithMaxAttempts(10)
                .WithTargetWordsAddress("Custom")
                .Build();

            Assert.AreEqual(8, config.WordLength);
            Assert.AreEqual(10, config.MaxAttempts);
            Assert.AreEqual("Custom", config.TargetWordsAddress);
            Assert.AreEqual("ValidGuessWords", config.ValidGuessWordsAddress);
        }

        [Test]
        public void Build_WithAllCustomValues_CreatesFullyCustomConfig()
        {
            var config = new ProjectConfigBuilder()
                .WithWordLength(9)
                .WithMaxAttempts(12)
                .WithTargetWordsAddress("Target")
                .WithValidGuessWordsAddress("Guess")
                .WithStatisticsSaveKey("Stats")
                .WithGameStateSaveKey("GameState")
                .Build();

            Assert.AreEqual(9, config.WordLength);
            Assert.AreEqual(12, config.MaxAttempts);
            Assert.AreEqual("Target", config.TargetWordsAddress);
            Assert.AreEqual("Guess", config.ValidGuessWordsAddress);
            Assert.AreEqual("Stats", config.StatisticsSaveKey);
            Assert.AreEqual("GameState", config.GameStateSaveKey);
        }

        [Test]
        public void Build_ChainingMethods_ReturnsBuilder()
        {
            var builder = new ProjectConfigBuilder()
                .WithWordLength(5)
                .WithMaxAttempts(6);

            Assert.IsInstanceOf<ProjectConfigBuilder>(builder);
        }
    }

    public class MockLogService : ILogService
    {
        public void LogDebug(string message) {}
        public void LogInfo(string message) {}
        public void LogWarning(string message) {}
        public void LogError(string message) {}
        public void LogException(string message, System.Exception exception) {}
        public void SetLogLevel(LogLevel level) {}
    }
}