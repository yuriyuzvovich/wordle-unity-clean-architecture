using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Wordle.Application.Tests.Mocks;
using Wordle.Application.UseCases;
using Wordle.Core.Entities;
using Wordle.Core.Events;
using Wordle.Core.ValueObjects;

namespace Wordle.Application.Tests.UseCases
{
    [TestFixture]
    public class SubmitGuessUseCaseTests
    {
        private MockEventBus _eventBus;
        private MockWordValidator _wordValidator;
        private MockGuessEvaluator _guessEvaluator;
        private MockGameStateRepository _gameStateRepository;
        private SubmitGuessUseCase _useCase;
        private MockGameRules _gameRules;

        [SetUp]
        public void SetUp()
        {
            _eventBus = new MockEventBus();
            _wordValidator = new MockWordValidator();
            _guessEvaluator = new MockGuessEvaluator();
            _gameStateRepository = new MockGameStateRepository();
            _gameRules = new MockGameRules();

            _wordValidator.AddValidWords("hello", "world", "tests", "guess", "valid");

            _useCase = new SubmitGuessUseCase(
                _eventBus,
                _wordValidator,
                _guessEvaluator,
                _gameStateRepository,
                _gameRules
            );
        }

        [Test]
        public void Constructor_NullEventBus_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SubmitGuessUseCase(
                    null, _wordValidator, _guessEvaluator,
                    _gameStateRepository,
                    _gameRules
                )
            );
        }

        [Test]
        public void Constructor_NullWordValidator_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SubmitGuessUseCase(
                    _eventBus, null, _guessEvaluator,
                    _gameStateRepository,
                    _gameRules
                )
            );
        }

        [Test]
        public void Constructor_NullGuessEvaluator_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SubmitGuessUseCase(
                    _eventBus, _wordValidator, null,
                    _gameStateRepository,
                    _gameRules
                )
            );
        }

        [Test]
        public void Constructor_NullGameStateRepository_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new SubmitGuessUseCase(
                    _eventBus,
                    _wordValidator,
                    _guessEvaluator,
                    null,
                    _gameRules
                )
            );
        }

        [Test]
        public async Task ExecuteAsync_NullGameState_ReturnsFailure()
        {
            var result = await _useCase.ExecuteAsync(null, "hello");

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Game state is null", result.Message);
        }

        [Test]
        public async Task ExecuteAsync_NullGuessWord_ReturnsFailure()
        {
            var gameState = new GameState(new Word("hello"));

            var result = await _useCase.ExecuteAsync(gameState, null);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Guess word cannot be empty", result.Message);
        }

        [Test]
        public async Task ExecuteAsync_EmptyGuessWord_ReturnsFailure()
        {
            var gameState = new GameState(new Word("hello"));

            var result = await _useCase.ExecuteAsync(gameState, "");

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Guess word cannot be empty", result.Message);
        }

        [Test]
        public async Task ExecuteAsync_GameOver_ReturnsFailure()
        {
            var gameState = new GameState(new Word("hello"));
            for (int i = 0; i < 6; i++)
            {
                var evaluations = CreateEvaluations('w', LetterEvaluation.Absent);
                gameState.AddGuess(new Guess(new Word("world"), evaluations));
            }

            var result = await _useCase.ExecuteAsync(gameState, "tests");

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Cannot make a guess. Game is over or no attempts remaining.", result.Message);
        }

        [Test]
        public async Task ExecuteAsync_InvalidWord_ReturnsFailure()
        {
            var gameState = new GameState(new Word("hello"));

            var result = await _useCase.ExecuteAsync(gameState, "zzzzz");

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Word is not in the word list", result.Message);
        }

        [Test]
        public async Task ExecuteAsync_ValidGuess_ReturnsSuccess()
        {
            var gameState = new GameState(new Word("hello"));

            var result = await _useCase.ExecuteAsync(gameState, "world");

            Assert.IsTrue(result.Success);
            Assert.AreEqual("Guess submitted successfully", result.Message);
        }

        [Test]
        public async Task ExecuteAsync_ValidGuess_AddsGuessToGameState()
        {
            var gameState = new GameState(new Word("hello"));

            await _useCase.ExecuteAsync(gameState, "world");

            Assert.AreEqual(1, gameState.CurrentAttempt);
            Assert.AreEqual(1, gameState.Guesses.Count);
            Assert.AreEqual("WORLD", gameState.Guesses[0].GuessedWord.Value);
        }

        [Test]
        public async Task ExecuteAsync_ValidGuess_PublishesGuessSubmittedEvent()
        {
            var gameState = new GameState(new Word("hello"));

            await _useCase.ExecuteAsync(gameState, "world");

            Assert.IsTrue(_eventBus.WasEventPublished<GuessSubmittedEvent>());
            var evt = _eventBus.GetPublishedEvent<GuessSubmittedEvent>();
            Assert.AreEqual("world", evt.GuessWord);
            Assert.AreEqual(5, evt.AttemptsRemaining);
            Assert.IsFalse(evt.IsCorrect);
        }

        [Test]
        public async Task ExecuteAsync_ValidGuess_SavesGameState()
        {
            var gameState = new GameState(new Word("hello"));

            await _useCase.ExecuteAsync(gameState, "world");

            var savedState = _gameStateRepository.GetSavedState();
            Assert.IsNotNull(savedState);
            Assert.AreEqual(1, savedState.CurrentAttempt);
        }

        [Test]
        public async Task ExecuteAsync_CorrectGuess_PublishesGameWonEvent()
        {
            var gameState = new GameState(new Word("hello"));

            await _useCase.ExecuteAsync(gameState, "hello");

            Assert.IsTrue(_eventBus.WasEventPublished<GameWonEvent>());
            var evt = _eventBus.GetPublishedEvent<GameWonEvent>();
            Assert.AreEqual("HELLO", evt.TargetWord);
            Assert.AreEqual(1, evt.AttemptsUsed);
        }

        [Test]
        public async Task ExecuteAsync_CorrectGuess_ReturnsCorrectResult()
        {
            var gameState = new GameState(new Word("hello"));

            var result = await _useCase.ExecuteAsync(gameState, "hello");

            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.IsCorrect);
            Assert.AreEqual(GameStatus.Won, result.GameStatus);
        }

        [Test]
        public async Task ExecuteAsync_LastIncorrectGuess_PublishesGameLostEvent()
        {
            var gameState = new GameState(new Word("hello"));
            for (int i = 0; i < 5; i++)
            {
                var evaluations = CreateEvaluations('w', LetterEvaluation.Absent);
                gameState.AddGuess(new Guess(new Word("world"), evaluations));
            }

            await _useCase.ExecuteAsync(gameState, "tests");

            Assert.IsTrue(_eventBus.WasEventPublished<GameLostEvent>());
            var evt = _eventBus.GetPublishedEvent<GameLostEvent>();
            Assert.AreEqual("HELLO", evt.TargetWord);
            Assert.AreEqual(6, evt.AttemptsUsed);
        }

        [Test]
        public async Task ExecuteAsync_LastIncorrectGuess_ReturnsLostStatus()
        {
            var gameState = new GameState(new Word("hello"));
            for (int i = 0; i < 5; i++)
            {
                var evaluations = CreateEvaluations('w', LetterEvaluation.Absent);
                gameState.AddGuess(new Guess(new Word("world"), evaluations));
            }

            var result = await _useCase.ExecuteAsync(gameState, "tests");

            Assert.IsTrue(result.Success);
            Assert.IsFalse(result.IsCorrect);
            Assert.AreEqual(GameStatus.Lost, result.GameStatus);
            Assert.AreEqual(0, result.AttemptsRemaining);
        }

        [Test]
        public async Task ExecuteAsync_InvalidWordLength_ReturnsFailure()
        {
            var gameState = new GameState(new Word("hello"));

            var result = await _useCase.ExecuteAsync(gameState, "test");

            Assert.IsFalse(result.Success);
        }

        [Test]
        public async Task ExecuteAsync_MultipleGuesses_PublishesEventsInOrder()
        {
            var gameState = new GameState(new Word("hello"));

            await _useCase.ExecuteAsync(gameState, "world");
            await _useCase.ExecuteAsync(gameState, "tests");

            Assert.AreEqual(2, _eventBus.GetPublishedEventCount<GuessSubmittedEvent>());
        }

        [Test]
        public async Task ExecuteAsync_WonGame_PublishesBothGuessSubmittedAndGameWonEvents()
        {
            var gameState = new GameState(new Word("hello"));

            await _useCase.ExecuteAsync(gameState, "hello");

            Assert.IsTrue(_eventBus.WasEventPublished<GuessSubmittedEvent>());
            Assert.IsTrue(_eventBus.WasEventPublished<GameWonEvent>());
        }

        [Test]
        public async Task ExecuteAsync_LostGame_PublishesBothGuessSubmittedAndGameLostEvents()
        {
            var gameState = new GameState(new Word("hello"));
            for (int i = 0; i < 5; i++)
            {
                var evaluations = CreateEvaluations('w', LetterEvaluation.Absent);
                gameState.AddGuess(new Guess(new Word("world"), evaluations));
            }

            await _useCase.ExecuteAsync(gameState, "tests");

            Assert.IsTrue(_eventBus.WasEventPublished<GuessSubmittedEvent>());
            Assert.IsTrue(_eventBus.WasEventPublished<GameLostEvent>());
        }

        private LetterPosition[] CreateEvaluations(char letter, LetterEvaluation evaluation)
        {
            var evaluations = new LetterPosition[Word.WORD_LENGTH];
            for (int i = 0; i < Word.WORD_LENGTH; i++)
            {
                evaluations[i] = new LetterPosition(letter, i, evaluation);
            }
            return evaluations;
        }
    }
}