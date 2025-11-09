using NUnit.Framework;
using System;
using Wordle.Core.Entities;
using Wordle.Core.ValueObjects;

namespace Tests
{
    [TestFixture]
    public class GuessTests
    {
        [Test]
        public void Constructor_ValidInputs_CreatesGuess()
        {
            var word = new Word("hello");
            var evaluations = new LetterPosition[]
            {
                new LetterPosition('H', 0, LetterEvaluation.Correct),
                new LetterPosition('E', 1, LetterEvaluation.Present),
                new LetterPosition('L', 2, LetterEvaluation.Absent),
                new LetterPosition('L', 3, LetterEvaluation.Absent),
                new LetterPosition('O', 4, LetterEvaluation.Correct)
            };

            var guess = new Guess(word, evaluations);

            Assert.AreEqual(word, guess.GuessedWord);
            Assert.AreEqual(evaluations, guess.Evaluations);
            Assert.That(guess.Timestamp, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(1)));
        }

        [Test]
        public void Constructor_NullEvaluations_ThrowsArgumentNullException()
        {
            var word = new Word("hello");

            Assert.Throws<ArgumentNullException>(() => new Guess(word, null));
        }

        [Test]
        public void Constructor_WrongEvaluationsLength_ThrowsArgumentException()
        {
            var word = new Word("hello");
            var evaluations = new LetterPosition[]
            {
                new LetterPosition('H', 0, LetterEvaluation.Correct),
                new LetterPosition('E', 1, LetterEvaluation.Present)
            };

            Assert.Throws<ArgumentException>(() => new Guess(word, evaluations));
        }

        [Test]
        public void IsCorrect_AllLettersCorrect_ReturnsTrue()
        {
            var word = new Word("hello");
            var evaluations = new LetterPosition[]
            {
                new LetterPosition('H', 0, LetterEvaluation.Correct),
                new LetterPosition('E', 1, LetterEvaluation.Correct),
                new LetterPosition('L', 2, LetterEvaluation.Correct),
                new LetterPosition('L', 3, LetterEvaluation.Correct),
                new LetterPosition('O', 4, LetterEvaluation.Correct)
            };

            var guess = new Guess(word, evaluations);

            Assert.IsTrue(guess.IsCorrect());
        }

        [Test]
        public void IsCorrect_SomeLettersIncorrect_ReturnsFalse()
        {
            var word = new Word("hello");
            var evaluations = new LetterPosition[]
            {
                new LetterPosition('H', 0, LetterEvaluation.Correct),
                new LetterPosition('E', 1, LetterEvaluation.Present),
                new LetterPosition('L', 2, LetterEvaluation.Correct),
                new LetterPosition('L', 3, LetterEvaluation.Correct),
                new LetterPosition('O', 4, LetterEvaluation.Correct)
            };

            var guess = new Guess(word, evaluations);

            Assert.IsFalse(guess.IsCorrect());
        }

        [Test]
        public void IsCorrect_AllLettersAbsent_ReturnsFalse()
        {
            var word = new Word("hello");
            var evaluations = new LetterPosition[]
            {
                new LetterPosition('H', 0, LetterEvaluation.Absent),
                new LetterPosition('E', 1, LetterEvaluation.Absent),
                new LetterPosition('L', 2, LetterEvaluation.Absent),
                new LetterPosition('L', 3, LetterEvaluation.Absent),
                new LetterPosition('O', 4, LetterEvaluation.Absent)
            };

            var guess = new Guess(word, evaluations);

            Assert.IsFalse(guess.IsCorrect());
        }

        [Test]
        public void ToString_ReturnsFormattedString()
        {
            var word = new Word("hello");
            var evaluations = new LetterPosition[]
            {
                new LetterPosition('H', 0, LetterEvaluation.Correct),
                new LetterPosition('E', 1, LetterEvaluation.Correct),
                new LetterPosition('L', 2, LetterEvaluation.Correct),
                new LetterPosition('L', 3, LetterEvaluation.Correct),
                new LetterPosition('O', 4, LetterEvaluation.Correct)
            };

            var guess = new Guess(word, evaluations);
            var result = guess.ToString();

            Assert.That(result, Does.Contain("HELLO"));
        }
    }
}
