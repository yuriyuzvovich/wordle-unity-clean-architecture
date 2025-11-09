using NUnit.Framework;
using System;
using Wordle.Core.Entities;
using Wordle.Core.ValueObjects;

namespace Tests
{
    [TestFixture]
    public class GameStateTests
    {
        [Test]
        public void Constructor_ValidInputs_CreatesGameState()
        {
            var targetWord = new Word("hello");

            var gameState = new GameState(targetWord);

            Assert.AreEqual(targetWord, gameState.TargetWord);
            Assert.AreEqual(GameState.DEFAULT_MAX_ATTEMPTS, gameState.MaxAttempts);
            Assert.AreEqual(0, gameState.CurrentAttempt);
            Assert.AreEqual(GameState.DEFAULT_MAX_ATTEMPTS, gameState.AttemptsRemaining);
            Assert.AreEqual(GameStatus.Playing, gameState.Status);
            Assert.That(gameState.StartTime, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(1)));
        }

        [Test]
        public void Constructor_CustomMaxAttempts_CreatesGameState()
        {
            var targetWord = new Word("hello");
            var maxAttempts = 10;

            var gameState = new GameState(targetWord, maxAttempts);

            Assert.AreEqual(maxAttempts, gameState.MaxAttempts);
            Assert.AreEqual(maxAttempts, gameState.AttemptsRemaining);
        }

        [Test]
        public void Constructor_ZeroMaxAttempts_ThrowsArgumentException()
        {
            var targetWord = new Word("hello");

            Assert.Throws<ArgumentException>(() => new GameState(targetWord, 0));
        }

        [Test]
        public void Constructor_NegativeMaxAttempts_ThrowsArgumentException()
        {
            var targetWord = new Word("hello");

            Assert.Throws<ArgumentException>(() => new GameState(targetWord, -1));
        }

        [Test]
        public void CanMakeGuess_NewGame_ReturnsTrue()
        {
            var targetWord = new Word("hello");
            var gameState = new GameState(targetWord);

            Assert.IsTrue(gameState.CanMakeGuess());
        }

        [Test]
        public void AddGuess_ValidGuess_AddsGuessToList()
        {
            var targetWord = new Word("hello");
            var gameState = new GameState(targetWord);
            var guess = CreateGuess("world", LetterEvaluation.Absent);

            gameState.AddGuess(guess);

            Assert.AreEqual(1, gameState.CurrentAttempt);
            Assert.AreEqual(GameState.DEFAULT_MAX_ATTEMPTS - 1, gameState.AttemptsRemaining);
            Assert.AreEqual(GameStatus.Playing, gameState.Status);
        }

        [Test]
        public void AddGuess_CorrectGuess_SetsStatusToWon()
        {
            var targetWord = new Word("hello");
            var gameState = new GameState(targetWord);
            var guess = CreateGuess("hello", LetterEvaluation.Correct);

            gameState.AddGuess(guess);

            Assert.AreEqual(GameStatus.Won, gameState.Status);
        }

        [Test]
        public void AddGuess_MaxAttemptsReached_SetsStatusToLost()
        {
            var targetWord = new Word("hello");
            var gameState = new GameState(targetWord);

            for (int i = 0; i < GameState.DEFAULT_MAX_ATTEMPTS; i++)
            {
                var guess = CreateGuess("world", LetterEvaluation.Absent);
                gameState.AddGuess(guess);
            }

            Assert.AreEqual(GameStatus.Lost, gameState.Status);
            Assert.AreEqual(0, gameState.AttemptsRemaining);
        }

        [Test]
        public void AddGuess_AfterGameWon_ThrowsInvalidOperationException()
        {
            var targetWord = new Word("hello");
            var gameState = new GameState(targetWord);
            var correctGuess = CreateGuess("hello", LetterEvaluation.Correct);
            gameState.AddGuess(correctGuess);

            var anotherGuess = CreateGuess("world", LetterEvaluation.Absent);

            Assert.Throws<InvalidOperationException>(() => gameState.AddGuess(anotherGuess));
        }

        [Test]
        public void AddGuess_AfterGameLost_ThrowsInvalidOperationException()
        {
            var targetWord = new Word("hello");
            var gameState = new GameState(targetWord);

            for (int i = 0; i < GameState.DEFAULT_MAX_ATTEMPTS; i++)
            {
                var guess = CreateGuess("world", LetterEvaluation.Absent);
                gameState.AddGuess(guess);
            }

            var anotherGuess = CreateGuess("world", LetterEvaluation.Absent);

            Assert.Throws<InvalidOperationException>(() => gameState.AddGuess(anotherGuess));
        }

        [Test]
        public void GetLastGuess_AfterAddingGuess_ReturnsLastGuess()
        {
            var targetWord = new Word("hello");
            var gameState = new GameState(targetWord);
            var guess1 = CreateGuess("world", LetterEvaluation.Absent);
            var guess2 = CreateGuess("earth", LetterEvaluation.Absent);

            gameState.AddGuess(guess1);
            gameState.AddGuess(guess2);

            var lastGuess = gameState.GetLastGuess();

            Assert.AreEqual(guess2.GuessedWord, lastGuess.GuessedWord);
        }

        [Test]
        public void GetLastGuess_NoGuesses_ThrowsInvalidOperationException()
        {
            var targetWord = new Word("hello");
            var gameState = new GameState(targetWord);

            Assert.Throws<InvalidOperationException>(() => gameState.GetLastGuess());
        }

        [Test]
        public void HasGuesses_NoGuesses_ReturnsFalse()
        {
            var targetWord = new Word("hello");
            var gameState = new GameState(targetWord);

            Assert.IsFalse(gameState.HasGuesses());
        }

        [Test]
        public void HasGuesses_WithGuesses_ReturnsTrue()
        {
            var targetWord = new Word("hello");
            var gameState = new GameState(targetWord);
            var guess = CreateGuess("world", LetterEvaluation.Absent);

            gameState.AddGuess(guess);

            Assert.IsTrue(gameState.HasGuesses());
        }

        [Test]
        public void Guesses_ReturnsReadOnlyList()
        {
            var targetWord = new Word("hello");
            var gameState = new GameState(targetWord);
            var guess = CreateGuess("world", LetterEvaluation.Absent);

            gameState.AddGuess(guess);

            var guesses = gameState.Guesses;

            Assert.AreEqual(1, guesses.Count);
            Assert.AreEqual(guess.GuessedWord, guesses[0].GuessedWord);
        }

        [Test]
        public void CanMakeGuess_AfterGameWon_ReturnsFalse()
        {
            var targetWord = new Word("hello");
            var gameState = new GameState(targetWord);
            var correctGuess = CreateGuess("hello", LetterEvaluation.Correct);

            gameState.AddGuess(correctGuess);

            Assert.IsFalse(gameState.CanMakeGuess());
        }

        [Test]
        public void CanMakeGuess_AfterGameLost_ReturnsFalse()
        {
            var targetWord = new Word("hello");
            var gameState = new GameState(targetWord);

            for (int i = 0; i < GameState.DEFAULT_MAX_ATTEMPTS; i++)
            {
                var guess = CreateGuess("world", LetterEvaluation.Absent);
                gameState.AddGuess(guess);
            }

            Assert.IsFalse(gameState.CanMakeGuess());
        }

        private Guess CreateGuess(string word, LetterEvaluation evaluation)
        {
            var guessedWord = new Word(word);
            var evaluations = new LetterPosition[Word.WORD_LENGTH];

            for (int i = 0; i < Word.WORD_LENGTH; i++)
            {
                evaluations[i] = new LetterPosition(guessedWord[i], i, evaluation);
            }

            return new Guess(guessedWord, evaluations);
        }
    }
}
