using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Wordle.Application.Events;
using Wordle.Application.Interfaces;
using Wordle.Core.Entities;
using Wordle.Core.Interfaces;

namespace Wordle.Infrastructure.Persistence
{
    public class WordRepository : IWordRepository
    {
        private readonly IAssetService _assetService;
        private readonly ILogService _logService;
        private readonly IProjectConfigService _configService;
        private readonly IEventBus _eventBus;
        private readonly ILocalizationService _localizationService;

        private string[] _targetWords;
        private string[] _validGuessWords;
        private readonly System.Random _random;

        public bool IsInitialized { get; private set; }

        public WordRepository(
            IAssetService assetService,
            ILogService logService,
            IProjectConfigService configService,
            IEventBus eventBus,
            ILocalizationService localizationService
        )
        {
            _assetService = assetService ?? throw new ArgumentNullException(nameof(assetService));
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _random = new System.Random();
            IsInitialized = false;

            _eventBus.Subscribe<LanguageChangedEvent>(OnLanguageChanged);
        }

        private void OnLanguageChanged(LanguageChangedEvent evt)
        {
            _logService.LogInfo($"WordRepository: Language changed to '{evt.LanguageCode}', reloading word lists...");
            ReloadWordsAsync(evt.LanguageCode).Forget();
        }

        public async UniTask InitializeAsync()
        {
            if (IsInitialized)
            {
                _logService.LogWarning("WordRepository is already initialized");
                return;
            }

            var currentLanguage = _localizationService.CurrentLanguage;
            _logService.LogInfo($"Initializing WordRepository for language: {currentLanguage}");

            await ReloadWordsAsync(currentLanguage);
            IsInitialized = true;
        }

        public async UniTask ReloadWordsAsync(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
            {
                _logService.LogError("WordRepository: Language code cannot be null or empty");
                return;
            }

            try
            {
                _logService.LogInfo($"Loading word lists for language '{languageCode}' from Addressables...");

                var targetWordsAddress = $"{languageCode}/{_configService.TargetWordsAddress}";
                var validGuessWordsAddress = $"{languageCode}/{_configService.ValidGuessWordsAddress}";

                var targetWordsAsset = await _assetService.LoadAssetAsync<TextAsset>(targetWordsAddress);
                _targetWords = ParseWordList(targetWordsAsset.text);
                _logService.LogInfo($"Loaded {_targetWords.Length} target words for '{languageCode}'");

                var validGuessWordsAsset = await _assetService.LoadAssetAsync<TextAsset>(validGuessWordsAddress);
                _validGuessWords = ParseWordList(validGuessWordsAsset.text);
                _logService.LogInfo($"Loaded {_validGuessWords.Length} valid guess words for '{languageCode}'");

                _eventBus.Publish(new WordsReloadedEvent(languageCode, _targetWords.Length, _validGuessWords.Length));
                _logService.LogInfo($"WordRepository: Words reloaded successfully for '{languageCode}'");
            }
            catch (Exception ex)
            {
                _logService.LogError($"Failed to reload words for language '{languageCode}': {ex.Message}");
                throw;
            }
        }

        public UniTask<Word> GetRandomTargetWordAsync()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("WordRepository is not initialized. Call InitializeAsync first.");
            }

            if (_targetWords == null || _targetWords.Length == 0)
            {
                throw new InvalidOperationException("No target words available");
            }

            var randomIndex = _random.Next(_targetWords.Length);
            var randomWord = _targetWords[randomIndex];
            var word = new Word(randomWord);

            _logService.LogInfo($"Selected random target word (index: {randomIndex})");
            return UniTask.FromResult(word);
        }

        public UniTask<string[]> GetValidGuessWordsAsync()
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("WordRepository is not initialized. Call InitializeAsync first.");
            }

            if (_validGuessWords == null || _validGuessWords.Length == 0)
            {
                throw new InvalidOperationException("No valid guess words available");
            }

            return UniTask.FromResult(_validGuessWords);
        }

        private string[] ParseWordList(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Array.Empty<string>();
            }

            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var wordList = new List<string>();

            foreach (var line in lines)
            {
                var trimmed = line.Trim().ToUpper();
                if (!string.IsNullOrEmpty(trimmed) && trimmed.Length == _configService.WordLength)
                {
                    wordList.Add(trimmed);
                }
            }

            return wordList.ToArray();
        }
    }
}