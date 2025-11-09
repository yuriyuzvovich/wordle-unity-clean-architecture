using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Wordle.Application.Tests.Mocks;
using Wordle.Application.UseCases;
using Wordle.Core.Entities;

namespace Wordle.Application.Tests.UseCases
{
    [TestFixture]
    public class GetGameStateUseCaseTests
    {
        private MockGameStateRepository _repository;
        private GetGameStateUseCase _useCase;

        [SetUp]
        public void SetUp()
        {
            _repository = new MockGameStateRepository();
            _useCase = new GetGameStateUseCase(_repository);
        }

        [Test]
        public void Constructor_NullRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GetGameStateUseCase(null));
        }

        [Test]
        public async Task ExecuteAsync_NoSavedState_ReturnsFailure()
        {
            var result = await _useCase.ExecuteAsync();

            Assert.IsFalse(result.Success);
            Assert.AreEqual("No saved game state found", result.Message);
            Assert.IsNull(result.GameState);
        }

        [Test]
        public async Task ExecuteAsync_WithSavedState_ReturnsSuccess()
        {
            var targetWord = new Word("hello");
            var gameState = new GameState(targetWord);
            await _repository.SaveAsync(gameState);

            var result = await _useCase.ExecuteAsync();

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Game state loaded successfully", result.Message);
            Assert.IsNotNull(result.GameState);
        }

        [Test]
        public async Task ExecuteAsync_WithSavedState_ReturnsCorrectGameStateDTO()
        {
            var targetWord = new Word("hello");
            var gameState = new GameState(targetWord);
            await _repository.SaveAsync(gameState);

            var result = await _useCase.ExecuteAsync();

            Assert.IsTrue(result.GameState.HasValue);
            var dto = result.GameState.Value;
            Assert.AreEqual("HELLO", dto.TargetWord);
            Assert.AreEqual(6, dto.MaxAttempts);
            Assert.AreEqual(0, dto.CurrentAttempt);
            Assert.AreEqual(6, dto.AttemptsRemaining);
            Assert.AreEqual(GameStatus.Playing, dto.Status);
        }

        [Test]
        public async Task ExecuteAsync_RepositoryThrows_ThrowsException()
        {
            var targetWord = new Word("hello");
            var gameState = new GameState(targetWord);
            await _repository.SaveAsync(gameState);
            _repository.ThrowOnLoad = true;

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _useCase.ExecuteAsync());
        }
    }
}
