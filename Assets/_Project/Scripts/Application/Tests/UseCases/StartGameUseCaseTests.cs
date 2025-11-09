using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Wordle.Application.Tests.Mocks;
using Wordle.Application.UseCases;
using Wordle.Application.Interfaces;
using Wordle.Core.Events;

namespace Wordle.Application.Tests.UseCases
{
    [TestFixture]
    public class StartGameUseCaseTests
    {
        private MockEventBus _eventBus;
        private IProjectConfigService _configService;
        private MockGameStateRepository _gameStateRepository;
        private StartGameUseCase _useCase;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new MockEventBus();
            _configService = new MockProjectConfigService();
            _gameStateRepository = new MockGameStateRepository();
            _useCase = new StartGameUseCase(_eventBus, _configService, _gameStateRepository);
        }

        [Test]
        public void Constructor_NullEventBus_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new StartGameUseCase(null, _configService, _gameStateRepository));
        }

        [Test]
        public void Constructor_NullConfigService_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new StartGameUseCase(_eventBus, null, _gameStateRepository));
        }

        [Test]
        public void Constructor_NullGameStateRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new StartGameUseCase(_eventBus, _configService, null));
        }

        [Test]
        public async Task ExecuteAsync_ValidTargetWord_ReturnsSuccess()
        {
            var result = await _useCase.ExecuteAsync("hello");

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Game started successfully", result.Message);
        }

        [Test]
        public async Task ExecuteAsync_ValidTargetWord_PublishesGameStartedEvent()
        {
            await _useCase.ExecuteAsync("hello");

            Assert.IsTrue(_eventBus.WasEventPublished<GameStartedEvent>());
            var evt = _eventBus.GetPublishedEvent<GameStartedEvent>();
            Assert.AreEqual("hello", evt.TargetWord);
            Assert.AreEqual(6, evt.MaxAttempts);
        }

        [Test]
        public async Task ExecuteAsync_NullTargetWord_ReturnsFailure()
        {
            var result = await _useCase.ExecuteAsync(null);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Target word cannot be empty", result.Message);
        }

        [Test]
        public async Task ExecuteAsync_EmptyTargetWord_ReturnsFailure()
        {
            var result = await _useCase.ExecuteAsync("");

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Target word cannot be empty", result.Message);
        }

        [Test]
        public async Task ExecuteAsync_TooShortTargetWord_ReturnsFailure()
        {
            var result = await _useCase.ExecuteAsync("test");

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Target word must be 5 letters", result.Message);
        }

        [Test]
        public async Task ExecuteAsync_TooLongTargetWord_ReturnsFailure()
        {
            var result = await _useCase.ExecuteAsync("testing");

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Target word must be 5 letters", result.Message);
        }

        [Test]
        public async Task ExecuteAsync_InvalidTargetWord_DoesNotPublishEvent()
        {
            await _useCase.ExecuteAsync("");

            Assert.IsFalse(_eventBus.WasEventPublished<GameStartedEvent>());
        }

        [Test]
        public async Task ExecuteAsync_UpperCaseTargetWord_ReturnsSuccess()
        {
            var result = await _useCase.ExecuteAsync("HELLO");

            Assert.IsTrue(result.Success);
        }

        [Test]
        public async Task ExecuteAsync_MixedCaseTargetWord_ReturnsSuccess()
        {
            var result = await _useCase.ExecuteAsync("HeLLo");

            Assert.IsTrue(result.Success);
        }

        [Test]
        public async Task ExecuteAsync_ValidTargetWord_SavesGameState()
        {
            await _useCase.ExecuteAsync("HELLO");

            var savedGameState = await _gameStateRepository.LoadAsync();
            Assert.IsNotNull(savedGameState);
            Assert.AreEqual("HELLO", savedGameState.TargetWord.Value);
            Assert.AreEqual(6, savedGameState.MaxAttempts);
        }

        // Simple mock config service for tests - moved to shared MockProjectConfigService in Tests/Mocks
    }
}