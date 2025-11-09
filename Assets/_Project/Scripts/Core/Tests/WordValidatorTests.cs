using NUnit.Framework;
using System;
using System.Collections.Generic;
using Wordle.Core.Entities;
using Wordle.Core.Services;

namespace Tests
{
    [TestFixture]
    public class WordValidatorTests
    {
        private List<string> _validWords;

        [SetUp]
        public void SetUp()
        {
            _validWords = new List<string>
            {
                "hello",
                "world",
                "tests",
                "valid",
                "words"
            };
        }

        [Test]
        public void Constructor_ValidWordList_CreatesValidator()
        {
            var validator = new WordValidator(_validWords);

            Assert.IsNotNull(validator);
        }

        [Test]
        public void Constructor_NullWordList_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new WordValidator(null));
        }

        [Test]
        public void Constructor_EmptyWordList_CreatesValidator()
        {
            var validator = new WordValidator(new List<string>());

            Assert.IsNotNull(validator);
        }

        [Test]
        public void IsValidWord_WordInList_ReturnsTrue()
        {
            var validator = new WordValidator(_validWords);
            var word = new Word("hello");

            var result = validator.IsValidWord(word);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsValidWord_WordNotInList_ReturnsFalse()
        {
            var validator = new WordValidator(_validWords);
            var word = new Word("abyss");

            var result = validator.IsValidWord(word);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsValidWord_String_WordInList_ReturnsTrue()
        {
            var validator = new WordValidator(_validWords);

            var result = validator.IsValidWord("hello");

            Assert.IsTrue(result);
        }

        [Test]
        public void IsValidWord_String_WordNotInList_ReturnsFalse()
        {
            var validator = new WordValidator(_validWords);

            var result = validator.IsValidWord("abyss");

            Assert.IsFalse(result);
        }

        [Test]
        public void IsValidWord_String_NullWord_ReturnsFalse()
        {
            var validator = new WordValidator(_validWords);

            var result = validator.IsValidWord((string)null);

            Assert.IsFalse(result);
        }

        [Test]
        public void IsValidWord_String_EmptyWord_ReturnsFalse()
        {
            var validator = new WordValidator(_validWords);

            var result = validator.IsValidWord("");

            Assert.IsFalse(result);
        }

        [Test]
        public void IsValidWord_String_WhitespaceWord_ReturnsFalse()
        {
            var validator = new WordValidator(_validWords);

            var result = validator.IsValidWord("   ");

            Assert.IsFalse(result);
        }

        [Test]
        public void IsValidWord_String_TooShortWord_ReturnsFalse()
        {
            var validator = new WordValidator(_validWords);

            var result = validator.IsValidWord("test");

            Assert.IsFalse(result);
        }

        [Test]
        public void IsValidWord_String_TooLongWord_ReturnsFalse()
        {
            var validator = new WordValidator(_validWords);

            var result = validator.IsValidWord("testing");

            Assert.IsFalse(result);
        }

        [Test]
        public void IsValidWord_CaseInsensitive_ReturnsTrue()
        {
            var validator = new WordValidator(_validWords);

            Assert.IsTrue(validator.IsValidWord("HELLO"));
            Assert.IsTrue(validator.IsValidWord("Hello"));
            Assert.IsTrue(validator.IsValidWord("hElLo"));
        }

        [Test]
        public void IsValidWord_WordWithWhitespace_TrimsAndValidates()
        {
            var validator = new WordValidator(_validWords);

            var result = validator.IsValidWord("  hello  ");

            Assert.IsTrue(result);
        }

        [Test]
        public void Constructor_FiltersInvalidWords_OnlyAddsValidLengthWords()
        {
            var mixedWords = new List<string>
            {
                "hello",
                "hi",
                "toolong",
                "world"
            };

            var validator = new WordValidator(mixedWords);

            Assert.IsTrue(validator.IsValidWord("hello"));
            Assert.IsTrue(validator.IsValidWord("world"));
            Assert.IsFalse(validator.IsValidWord("hi"));
            Assert.IsFalse(validator.IsValidWord("toolong"));
        }

        [Test]
        public void Constructor_FiltersNullAndWhitespaceWords()
        {
            var mixedWords = new List<string>
            {
                "hello",
                null,
                "",
                "   ",
                "world"
            };

            var validator = new WordValidator(mixedWords);

            Assert.IsTrue(validator.IsValidWord("hello"));
            Assert.IsTrue(validator.IsValidWord("world"));
        }

        [Test]
        public void Constructor_TrimsWordsBeforeAdding()
        {
            var wordsWithWhitespace = new List<string>
            {
                "  hello  ",
                "world  ",
                "  tests"
            };

            var validator = new WordValidator(wordsWithWhitespace);

            Assert.IsTrue(validator.IsValidWord("hello"));
            Assert.IsTrue(validator.IsValidWord("world"));
            Assert.IsTrue(validator.IsValidWord("tests"));
        }

        [Test]
        public void Constructor_NormalizesToUppercase()
        {
            var lowerCaseWords = new List<string>
            {
                "hello",
                "world"
            };

            var validator = new WordValidator(lowerCaseWords);

            Assert.IsTrue(validator.IsValidWord("HELLO"));
            Assert.IsTrue(validator.IsValidWord("hello"));
        }

        [Test]
        public void IsValidWord_DuplicateWords_HandlesProperly()
        {
            var duplicateWords = new List<string>
            {
                "hello",
                "hello",
                "HELLO",
                "world"
            };

            var validator = new WordValidator(duplicateWords);

            Assert.IsTrue(validator.IsValidWord("hello"));
            Assert.IsTrue(validator.IsValidWord("world"));
        }

        [Test]
        public void UpdateWords_ValidWordList_UpdatesValidator()
        {
            var initialWords = new List<string> { "hello", "world" };
            var validator = new WordValidator(initialWords);

            var newWords = new List<string> { "tests", "valid" };
            validator.UpdateWords(newWords);

            Assert.IsTrue(validator.IsValidWord("tests"));
            Assert.IsTrue(validator.IsValidWord("valid"));
            Assert.IsFalse(validator.IsValidWord("hello"));
            Assert.IsFalse(validator.IsValidWord("world"));
        }

        [Test]
        public void UpdateWords_NullWordList_ThrowsArgumentNullException()
        {
            var validator = new WordValidator(_validWords);

            Assert.Throws<ArgumentNullException>(() => validator.UpdateWords(null));
        }

        [Test]
        public void UpdateWords_EmptyWordList_ClearsValidator()
        {
            var validator = new WordValidator(_validWords);

            validator.UpdateWords(new List<string>());

            Assert.IsFalse(validator.IsValidWord("hello"));
            Assert.IsFalse(validator.IsValidWord("world"));
        }

        [Test]
        public void UpdateWords_SimulateLanguageChange_UpdatesWordList()
        {
            var englishWords = new List<string> { "hello", "world", "tests" };
            var germanWords = new List<string> { "apfel", "baum", "haus" };

            var validator = new WordValidator(englishWords);
            Assert.IsTrue(validator.IsValidWord("hello"));
            Assert.IsFalse(validator.IsValidWord("apfel"));

            validator.UpdateWords(germanWords);
            Assert.IsTrue(validator.IsValidWord("apfel"));
            Assert.IsFalse(validator.IsValidWord("hello"));
        }

        [Test]
        public void UpdateWords_FiltersInvalidWords_OnlyAddsValidLengthWords()
        {
            var validator = new WordValidator(_validWords);

            var mixedWords = new List<string>
            {
                "apple",
                "hi",
                "toolong",
                "grape"
            };

            validator.UpdateWords(mixedWords);

            Assert.IsTrue(validator.IsValidWord("apple"));
            Assert.IsTrue(validator.IsValidWord("grape"));
            Assert.IsFalse(validator.IsValidWord("hi"));
            Assert.IsFalse(validator.IsValidWord("toolong"));
        }

        [Test]
        public void UpdateWords_TrimsAndNormalizes_HandlesCorrectly()
        {
            var validator = new WordValidator(_validWords);

            var newWords = new List<string>
            {
                "  apple  ",
                "GRAPE",
                "pEaCh"
            };

            validator.UpdateWords(newWords);

            Assert.IsTrue(validator.IsValidWord("apple"));
            Assert.IsTrue(validator.IsValidWord("APPLE"));
            Assert.IsTrue(validator.IsValidWord("grape"));
            Assert.IsTrue(validator.IsValidWord("peach"));
        }
    }
}
