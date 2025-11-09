using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Wordle.Core.Entities;
using Wordle.Core.Interfaces;

namespace Wordle.Application.Tests.Mocks
{
    public class MockWordRepository : IWordRepository
    {
        private readonly List<Word> _targetWords = new List<Word>();
        private int _currentIndex;

        public UniTask<string[]> GetValidGuessWordsAsync()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("Repository not initialized");
            }

            // Return the list of target words' string values as valid guesses
            var arr = _targetWords.Select(w => w.Value).ToArray();
            return UniTask.FromResult(arr);
        }

        public async UniTask InitializeAsync()
        {
            // Simulate initialization (no IO). If not initialized, mark as initialized.
            IsInitialized = true;
            await UniTask.Yield();
        }

        public bool IsInitialized { get; private set; }

        public MockWordRepository(bool isInitialized = true)
        {
            IsInitialized = isInitialized;
        }

        public void AddTargetWord(string word)
        {
            _targetWords.Add(new Word(word));
        }

        public UniTask<Word> GetRandomTargetWordAsync()
        {
            if (!IsInitialized || _targetWords.Count == 0)
            {
                throw new InvalidOperationException("Repository not initialized or no words available");
            }
            var word = _targetWords[_currentIndex % _targetWords.Count];
            _currentIndex++;
            return UniTask.FromResult(word);
        }

        public void SetInitialized(bool value)
        {
            IsInitialized = value;
        }

        public async UniTask ReloadWordsAsync(string languageCode)
        {
            await UniTask.Yield();
        }
    }
}
