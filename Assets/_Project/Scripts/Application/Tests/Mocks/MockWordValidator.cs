using System.Collections.Generic;
using Wordle.Core.Entities;
using Wordle.Core.Interfaces;

namespace Wordle.Application.Tests.Mocks
{
    public class MockWordValidator : IWordValidator
    {
        private readonly HashSet<string> _validWords = new HashSet<string>();

        public void AddValidWord(string word)
        {
            _validWords.Add(word.ToUpperInvariant());
        }

        public void AddValidWords(params string[] words)
        {
            foreach (var word in words)
            {
                AddValidWord(word);
            }
        }

        public bool IsValidWord(Word word)
        {
            return _validWords.Contains(word.Value.ToUpperInvariant());
        }

        public bool IsValidWord(string word)
        {
            if (string.IsNullOrEmpty(word) || word.Length != 5)
            {
                return false;
            }
            return _validWords.Contains(word.ToUpperInvariant());
        }
    }
}
