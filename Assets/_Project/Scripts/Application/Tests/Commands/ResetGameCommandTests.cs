using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using Wordle.Application.Commands;
using Wordle.Application.Interfaces;
using Wordle.Application.Tests.Mocks;
using Wordle.Application.UseCases;

namespace Wordle.Application.Tests.Commands
{
    [TestFixture]
    public class ResetGameCommandTests
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
        public void Constructor_NullUseCase_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ResetGameCommand(null));
        }

        [Test]
        public void CommandName_ReturnsCorrectName()
        {
            var command = new ResetGameCommand(_useCase);

            Assert.AreEqual("ResetGame", command.CommandName);
        }

        [Test]
        public void Priority_ReturnsHigh()
        {
            var command = new ResetGameCommand(_useCase);

            Assert.AreEqual(CommandPriority.High, command.Priority);
        }

        [Test]
        public void CanExecute_AlwaysReturnsTrue()
        {
            var command = new ResetGameCommand(_useCase);

            Assert.IsTrue(command.CanExecute());
        }

        [Test]
        public async Task ExecuteAsync_ValidRepository_CompletesSuccessfully()
        {
            var command = new ResetGameCommand(_useCase);

            await command.ExecuteAsync(CancellationToken.None);

            Assert.Pass();
        }

        [Test]
        public void ExecuteAsync_UninitializedRepository_ThrowsInvalidOperationException()
        {
            _wordRepository.SetInitialized(false);
            var command = new ResetGameCommand(_useCase);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await command.ExecuteAsync(CancellationToken.None));
        }

        [Test]
        public void OnComplete_DoesNotThrow()
        {
            var command = new ResetGameCommand(_useCase);

            Assert.DoesNotThrow(() => command.OnComplete());
        }

        [Test]
        public void OnFailed_DoesNotThrow()
        {
            var command = new ResetGameCommand(_useCase);

            Assert.DoesNotThrow(() => command.OnFailed(new Exception("Test exception")));
        }
    }
}
