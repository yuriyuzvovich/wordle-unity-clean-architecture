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
    public class StartGameCommandTests
    {
        private MockEventBus _eventBus;
        private MockProjectConfigService _configService;
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
        public void Constructor_NullUseCase_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new StartGameCommand(null, "hello"));
        }

        [Test]
        public void CommandName_ReturnsCorrectName()
        {
            var command = new StartGameCommand(_useCase, "hello");

            Assert.AreEqual("StartGame", command.CommandName);
        }

        [Test]
        public void Priority_ReturnsHigh()
        {
            var command = new StartGameCommand(_useCase, "hello");

            Assert.AreEqual(CommandPriority.High, command.Priority);
        }

        [Test]
        public void CanExecute_ValidTargetWord_ReturnsTrue()
        {
            var command = new StartGameCommand(_useCase, "hello");

            Assert.IsTrue(command.CanExecute());
        }

        [Test]
        public void CanExecute_NullTargetWord_ReturnsFalse()
        {
            var command = new StartGameCommand(_useCase, null);

            Assert.IsFalse(command.CanExecute());
        }

        [Test]
        public void CanExecute_EmptyTargetWord_ReturnsFalse()
        {
            var command = new StartGameCommand(_useCase, "");

            Assert.IsFalse(command.CanExecute());
        }

        [Test]
        public async Task ExecuteAsync_ValidTargetWord_CompletesSuccessfully()
        {
            var command = new StartGameCommand(_useCase, "hello");

            await command.ExecuteAsync(CancellationToken.None);

            Assert.Pass();
        }

        [Test]
        public void ExecuteAsync_InvalidTargetWord_ThrowsInvalidOperationException()
        {
            var command = new StartGameCommand(_useCase, "test");

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await command.ExecuteAsync(CancellationToken.None));
        }

        [Test]
        public void OnComplete_DoesNotThrow()
        {
            var command = new StartGameCommand(_useCase, "hello");

            Assert.DoesNotThrow(() => command.OnComplete());
        }

        [Test]
        public void OnFailed_DoesNotThrow()
        {
            var command = new StartGameCommand(_useCase, "hello");

            Assert.DoesNotThrow(() => command.OnFailed(new Exception("Test exception")));
        }
    }
}
