using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;
using Wordle.Application.Commands;
using Wordle.Application.Interfaces;
using Wordle.Application.Tests.Mocks;
using Wordle.Application.Tests.UseCases;
using Wordle.Application.UseCases;
using Wordle.Core.Entities;

namespace Wordle.Application.Tests.Commands
{
    [TestFixture]
    public class SubmitGuessCommandTests
    {
        private MockEventBus _eventBus;
        private MockWordValidator _wordValidator;
        private MockGuessEvaluator _guessEvaluator;
        private MockGameStateRepository _gameStateRepository;
        private SubmitGuessUseCase _useCase;
        private GameState _gameState;
        private MockGameRules _gameRules;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new MockEventBus();
            _wordValidator = new MockWordValidator();
            _guessEvaluator = new MockGuessEvaluator();
            _gameStateRepository = new MockGameStateRepository();
            _gameRules = new MockGameRules();

            _wordValidator.AddValidWords("hello", "world", "tests");

            _useCase = new SubmitGuessUseCase(
                _eventBus,
                _wordValidator,
                _guessEvaluator,
                _gameStateRepository,
                _gameRules
            );

            _gameState = new GameState(new Word("hello"));
        }

        [Test]
        public void Constructor_NullUseCase_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SubmitGuessCommand(null, _gameState, "hello")
            );
        }

        [Test]
        public void Constructor_NullGameState_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SubmitGuessCommand(_useCase, null, "hello")
            );
        }

        [Test]
        public void CommandName_ReturnsCorrectName()
        {
            var command = new SubmitGuessCommand(_useCase, _gameState, "hello");

            Assert.AreEqual("SubmitGuess", command.CommandName);
        }

        [Test]
        public void Priority_ReturnsHigh()
        {
            var command = new SubmitGuessCommand(_useCase, _gameState, "hello");

            Assert.AreEqual(CommandPriority.High, command.Priority);
        }

        [Test]
        public void CanExecute_ValidGuessAndGameActive_ReturnsTrue()
        {
            var command = new SubmitGuessCommand(_useCase, _gameState, "world");

            Assert.IsTrue(command.CanExecute());
        }

        [Test]
        public void CanExecute_NullGuessWord_ReturnsFalse()
        {
            var command = new SubmitGuessCommand(_useCase, _gameState, null);

            Assert.IsFalse(command.CanExecute());
        }

        [Test]
        public void CanExecute_EmptyGuessWord_ReturnsFalse()
        {
            var command = new SubmitGuessCommand(_useCase, _gameState, "");

            Assert.IsFalse(command.CanExecute());
        }

        [Test]
        public void CanExecute_GameOver_ReturnsFalse()
        {
            for (int i = 0; i < 6; i++)
            {
                var evaluations = new Core.ValueObjects.LetterPosition[5];
                for (int j = 0; j < 5; j++)
                {
                    evaluations[j] = new Core.ValueObjects.LetterPosition('w', j, Core.ValueObjects.LetterEvaluation.Absent);
                }
                _gameState.AddGuess(new Guess(new Word("world"), evaluations));
            }

            var command = new SubmitGuessCommand(_useCase, _gameState, "tests");

            Assert.IsFalse(command.CanExecute());
        }

        [Test]
        public async Task ExecuteAsync_ValidGuess_CompletesSuccessfully()
        {
            var command = new SubmitGuessCommand(_useCase, _gameState, "world");

            await command.ExecuteAsync(CancellationToken.None);

            Assert.Pass();
        }

        [Test]
        public void ExecuteAsync_InvalidGuess_ThrowsInvalidOperationException()
        {
            var command = new SubmitGuessCommand(_useCase, _gameState, "zzzzz");

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await command.ExecuteAsync(CancellationToken.None)
            );
        }

        [Test]
        public void OnComplete_DoesNotThrow()
        {
            var command = new SubmitGuessCommand(_useCase, _gameState, "world");

            Assert.DoesNotThrow(() => command.OnComplete());
        }

        [Test]
        public void OnFailed_DoesNotThrow()
        {
            var command = new SubmitGuessCommand(_useCase, _gameState, "world");

            Assert.DoesNotThrow(() => command.OnFailed(new Exception("Test exception")));
        }
    }
}