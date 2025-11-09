using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Wordle.Application.Tests.Mocks;
using Wordle.Application.UseCases;
using Wordle.Core.Entities;
using Wordle.Core.Events;

namespace Wordle.Application.Tests.UseCases
{
    [TestFixture]
    public class ResetGameUseCaseTests
    {
        private MockEventBus _eventBus;
        private MockGameStateRepository _gameStateRepository;
        private MockWordRepository _wordRepository;
        private ResetGameUseCase _useCase;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new MockEventBus();
            _gameStateRepository = new MockGameStateRepository();
            _wordRepository = new MockWordRepository();
            _wordRepository.AddTargetWord("hello");
            _useCase = new ResetGameUseCase(_eventBus, _gameStateRepository, _wordRepository);
        }

        [Test]
        public void Constructor_NullEventBus_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ResetGameUseCase(null, _gameStateRepository, _wordRepository));
        }

        [Test]
        public void Constructor_NullGameStateRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ResetGameUseCase(_eventBus, null, _wordRepository));
        }

        [Test]
        public void Constructor_NullWordRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ResetGameUseCase(_eventBus, _gameStateRepository, null));
        }

        [Test]
        public async Task ExecuteAsync_UninitializedWordRepository_ReturnsFailure()
        {
            _wordRepository.SetInitialized(false);

            var result = await _useCase.ExecuteAsync();

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Word repository is not initialized", result.Message);
            Assert.IsNull(result.GameState);
        }

        [Test]
        public async Task ExecuteAsync_Success_ClearsSavedState()
        {
            var oldGameState = new GameState(new Word("tests"));
            await _gameStateRepository.SaveAsync(oldGameState);

            await _useCase.ExecuteAsync();

            // After reset, there should be a NEW saved state (not the old one)
            var hasSavedState = await _gameStateRepository.HasSavedStateAsync();
            Assert.IsTrue(hasSavedState); // New game state should exist
            
            var newState = _gameStateRepository.GetSavedState();
            Assert.AreNotEqual(oldGameState.TargetWord, newState.TargetWord); // Should be a different word
        }

        [Test]
        public async Task ExecuteAsync_Success_CreatesNewGameState()
        {
            var result = await _useCase.ExecuteAsync();

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Game reset successfully", result.Message);
            Assert.IsNotNull(result.GameState);
        }

        [Test]
        public async Task ExecuteAsync_Success_SavesNewGameState()
        {
            await _useCase.ExecuteAsync();

            var savedState = _gameStateRepository.GetSavedState();
            Assert.IsNotNull(savedState);
            Assert.AreEqual("HELLO", savedState.TargetWord.Value);
        }

        [Test]
        public async Task ExecuteAsync_Success_PublishesGameStartedEvent()
        {
            await _useCase.ExecuteAsync();

            Assert.IsTrue(_eventBus.WasEventPublished<GameStartedEvent>());
            var evt = _eventBus.GetPublishedEvent<GameStartedEvent>();
            Assert.AreEqual("HELLO", evt.TargetWord);
            Assert.AreEqual(6, evt.MaxAttempts);
        }

        [Test]
        public async Task ExecuteAsync_Success_ReturnsGameStateWithCorrectProperties()
        {
            var result = await _useCase.ExecuteAsync();

            Assert.AreEqual("HELLO", result.GameState.TargetWord.Value);
            Assert.AreEqual(6, result.GameState.MaxAttempts);
            Assert.AreEqual(0, result.GameState.CurrentAttempt);
            Assert.AreEqual(GameStatus.Playing, result.GameState.Status);
        }

        [Test]
        public async Task ExecuteAsync_MultipleWords_GetsRandomWord()
        {
            _wordRepository.AddTargetWord("world");
            _wordRepository.AddTargetWord("tests");

            var result1 = await _useCase.ExecuteAsync();
            var result2 = await _useCase.ExecuteAsync();
            var result3 = await _useCase.ExecuteAsync();

            Assert.IsNotNull(result1.GameState);
            Assert.IsNotNull(result2.GameState);
            Assert.IsNotNull(result3.GameState);
        }
    }
}
