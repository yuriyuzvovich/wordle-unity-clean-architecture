using System;
using System.Collections.Generic;
using Wordle.Core.Entities;
using Wordle.Core.Interfaces;

namespace Wordle.Core.Services
{
    /// <summary>
    /// Domain service: Validates words against a word list.
    /// </summary>
    public class WordValidator : IWordValidator
    {
        private HashSet<string> _validWords;

        public WordValidator(IEnumerable<string> validWords)
        {
            if (validWords == null)
            {
                throw new ArgumentNullException(nameof(validWords));
            }

            _validWords = CreateWordSet(validWords);
        }

        public void UpdateWords(IEnumerable<string> validWords)
        {
            if (validWords == null)
            {
                throw new ArgumentNullException(nameof(validWords));
            }

            _validWords = CreateWordSet(validWords);
        }

        private HashSet<string> CreateWordSet(IEnumerable<string> validWords)
        {
            var wordSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var word in validWords)
            {
                if (!string.IsNullOrWhiteSpace(word) && word.Trim().Length == Word.WORD_LENGTH)
                {
                    wordSet.Add(word.Trim().ToUpper());
                }
            }
            return wordSet;
        }

        public bool IsValidWord(Word word)
        {
            return _validWords.Contains(word.Value);
        }

        public bool IsValidWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return false;
            }

            var normalized = word.Trim().ToUpper();
            return normalized.Length == Word.WORD_LENGTH && _validWords.Contains(normalized);
        }
    }
}
