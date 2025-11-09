using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Wordle.Application.Tests.Mocks;
using Wordle.Application.UseCases;

namespace Wordle.Application.Tests.UseCases
{
    [TestFixture]
    public class ValidateWordUseCaseTests
    {
        private MockWordValidator _wordValidator;
        private ValidateWordUseCase _useCase;

        [SetUp]
        public void SetUp()
        {
            _wordValidator = new MockWordValidator();
            _wordValidator.AddValidWords("hello", "world", "tests");
            _useCase = new ValidateWordUseCase(_wordValidator);
        }

        [Test]
        public void Constructor_NullWordValidator_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new ValidateWordUseCase(null));
        }

        [Test]
        public async Task ExecuteAsync_ValidWord_ReturnsValid()
        {
            var result = await _useCase.ExecuteAsync("hello");

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual("Word is valid", result.Message);
        }

        [Test]
        public async Task ExecuteAsync_InvalidWord_ReturnsInvalid()
        {
            var result = await _useCase.ExecuteAsync("zzzzz");

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Word is not in the word list", result.Message);
        }

        [Test]
        public async Task ExecuteAsync_NullWord_ReturnsInvalid()
        {
            var result = await _useCase.ExecuteAsync(null);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Word cannot be empty", result.Message);
        }

        [Test]
        public async Task ExecuteAsync_EmptyWord_ReturnsInvalid()
        {
            var result = await _useCase.ExecuteAsync("");

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Word cannot be empty", result.Message);
        }

        [Test]
        public async Task ExecuteAsync_TooShortWord_ReturnsInvalid()
        {
            var result = await _useCase.ExecuteAsync("test");

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Word must be 5 letters", result.Message);
        }

        [Test]
        public async Task ExecuteAsync_TooLongWord_ReturnsInvalid()
        {
            var result = await _useCase.ExecuteAsync("testing");

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Word must be 5 letters", result.Message);
        }

        [Test]
        public async Task ExecuteAsync_CaseInsensitive_ReturnsValid()
        {
            var result1 = await _useCase.ExecuteAsync("HELLO");
            var result2 = await _useCase.ExecuteAsync("HeLLo");

            Assert.IsTrue(result1.IsValid);
            Assert.IsTrue(result2.IsValid);
        }
    }
}
