using System;

namespace Wordle.Core.Entities
{
    /// <summary>
    /// Represents a valid Wordle word (5 letters, uppercase).
    /// Immutable entity with validation.
    /// </summary>
    public readonly struct Word
    {
        public const int WORD_LENGTH = 5;

        public readonly string Value;
        public readonly int Length;

        public Word(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Word cannot be null or empty", nameof(value));
            }

            var normalized = value.Trim().ToUpper();

            if (normalized.Length != WORD_LENGTH)
            {
                throw new ArgumentException($"Word must be exactly {WORD_LENGTH} letters, got {normalized.Length}", nameof(value));
            }

            if (!IsAllLetters(normalized))
            {
                throw new ArgumentException("Word must contain only letters", nameof(value));
            }

            Value = normalized;
            Length = WORD_LENGTH;
        }

        public char this[int index]
        {
            get
            {
                if (index < 0 || index >= WORD_LENGTH)
                {
                    throw new IndexOutOfRangeException($"Index must be between 0 and {WORD_LENGTH - 1}");
                }
                
                return Value[index];
            }
        }

        private static bool IsAllLetters(string str)
        {
            foreach (char c in str)
            {
                if (!char.IsLetter(c))
                {
                    return false;
                }
            }
            return true;
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            return obj is Word other && Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(Word left, Word right)
        {
            return left.Value == right.Value;
        }

        public static bool operator !=(Word left, Word right)
        {
            return !(left == right);
        }
    }
}
