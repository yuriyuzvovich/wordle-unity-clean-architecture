using NUnit.Framework;
using System;
using Wordle.Core.Entities;

namespace Tests
{
    [TestFixture]
    public class WordTests
    {
        [Test]
        public void Constructor_ValidWord_CreatesWord()
        {
            var word = new Word("hello");

            Assert.AreEqual("HELLO", word.Value);
            Assert.AreEqual(Word.WORD_LENGTH, word.Length);
        }

        [Test]
        public void Constructor_ValidWordWithWhitespace_TrimsAndCreatesWord()
        {
            var word = new Word("  hello  ");

            Assert.AreEqual("HELLO", word.Value);
        }

        [Test]
        public void Constructor_LowercaseWord_ConvertsToUppercase()
        {
            var word = new Word("world");

            Assert.AreEqual("WORLD", word.Value);
        }

        [Test]
        public void Constructor_MixedCaseWord_ConvertsToUppercase()
        {
            var word = new Word("WoRlD");

            Assert.AreEqual("WORLD", word.Value);
        }

        [Test]
        public void Constructor_NullWord_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Word(null));
        }

        [Test]
        public void Constructor_EmptyWord_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Word(""));
        }

        [Test]
        public void Constructor_WhitespaceWord_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Word("   "));
        }

        [Test]
        public void Constructor_TooShortWord_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Word("word"));
        }

        [Test]
        public void Constructor_TooLongWord_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Word("toolong"));
        }

        [Test]
        public void Constructor_WordWithNumbers_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Word("hel1o"));
        }

        [Test]
        public void Constructor_WordWithSpecialChars_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Word("hel!o"));
        }

        [Test]
        public void Indexer_ValidIndex_ReturnsCorrectLetter()
        {
            var word = new Word("hello");

            Assert.AreEqual('H', word[0]);
            Assert.AreEqual('E', word[1]);
            Assert.AreEqual('L', word[2]);
            Assert.AreEqual('L', word[3]);
            Assert.AreEqual('O', word[4]);
        }

        [Test]
        public void Indexer_NegativeIndex_ThrowsIndexOutOfRangeException()
        {
            var word = new Word("hello");

            Assert.Throws<IndexOutOfRangeException>(() => { var c = word[-1]; });
        }

        [Test]
        public void Indexer_IndexTooLarge_ThrowsIndexOutOfRangeException()
        {
            var word = new Word("hello");

            Assert.Throws<IndexOutOfRangeException>(() => { var c = word[5]; });
        }

        [Test]
        public void ToString_ReturnsValue()
        {
            var word = new Word("hello");

            Assert.AreEqual("HELLO", word.ToString());
        }

        [Test]
        public void Equals_SameWord_ReturnsTrue()
        {
            var word1 = new Word("hello");
            var word2 = new Word("hello");

            Assert.IsTrue(word1.Equals(word2));
        }

        [Test]
        public void Equals_DifferentWord_ReturnsFalse()
        {
            var word1 = new Word("hello");
            var word2 = new Word("world");

            Assert.IsFalse(word1.Equals(word2));
        }

        [Test]
        public void Equals_DifferentCase_ReturnsTrue()
        {
            var word1 = new Word("hello");
            var word2 = new Word("HELLO");

            Assert.IsTrue(word1.Equals(word2));
        }

        [Test]
        public void EqualityOperator_SameWord_ReturnsTrue()
        {
            var word1 = new Word("hello");
            var word2 = new Word("hello");

            Assert.IsTrue(word1 == word2);
        }

        [Test]
        public void EqualityOperator_DifferentWord_ReturnsFalse()
        {
            var word1 = new Word("hello");
            var word2 = new Word("world");

            Assert.IsTrue(word1 != word2);
        }

        [Test]
        public void GetHashCode_SameWord_ReturnsSameHashCode()
        {
            var word1 = new Word("hello");
            var word2 = new Word("hello");

            Assert.AreEqual(word1.GetHashCode(), word2.GetHashCode());
        }
    }
}
