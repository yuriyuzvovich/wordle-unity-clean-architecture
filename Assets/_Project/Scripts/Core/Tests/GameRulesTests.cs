using NUnit.Framework;
using Wordle.Core.Entities;
using Wordle.Core.Services;

namespace Tests
{
    [TestFixture]
    public class GameRulesTests
    {
        private GameRules _gameRules;

        [SetUp]
        public void SetUp()
        {
            _gameRules = new GameRules();
        }

        [Test]
        public void WordLength_ReturnsCorrectValue()
        {
            Assert.AreEqual(Word.WORD_LENGTH, _gameRules.WordLength);
            Assert.AreEqual(5, _gameRules.WordLength);
        }

        [Test]
        public void MaxAttempts_ReturnsCorrectValue()
        {
            Assert.AreEqual(GameState.DEFAULT_MAX_ATTEMPTS, _gameRules.MaxAttempts);
            Assert.AreEqual(6, _gameRules.MaxAttempts);
        }

        [Test]
        public void IsWordLengthValid_CorrectLength_ReturnsTrue()
        {
            var result = _gameRules.IsWordLengthValid(5);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsWordLengthValid_TooShort_ReturnsFalse()
        {
            var result = _gameRules.IsWordLengthValid(4);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsWordLengthValid_TooLong_ReturnsFalse()
        {
            var result = _gameRules.IsWordLengthValid(6);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsWordLengthValid_Zero_ReturnsFalse()
        {
            var result = _gameRules.IsWordLengthValid(0);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsWordLengthValid_Negative_ReturnsFalse()
        {
            var result = _gameRules.IsWordLengthValid(-1);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsAttemptCountValid_ValidCount_ReturnsTrue()
        {
            Assert.IsTrue(_gameRules.IsAttemptCountValid(1));
            Assert.IsTrue(_gameRules.IsAttemptCountValid(3));
            Assert.IsTrue(_gameRules.IsAttemptCountValid(6));
        }

        [Test]
        public void IsAttemptCountValid_MaxAttempts_ReturnsTrue()
        {
            var result = _gameRules.IsAttemptCountValid(6);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsAttemptCountValid_AboveMaxAttempts_ReturnsFalse()
        {
            var result = _gameRules.IsAttemptCountValid(7);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsAttemptCountValid_Zero_ReturnsFalse()
        {
            var result = _gameRules.IsAttemptCountValid(0);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsAttemptCountValid_Negative_ReturnsFalse()
        {
            var result = _gameRules.IsAttemptCountValid(-1);

            Assert.IsFalse(result);
        }
    }
}
