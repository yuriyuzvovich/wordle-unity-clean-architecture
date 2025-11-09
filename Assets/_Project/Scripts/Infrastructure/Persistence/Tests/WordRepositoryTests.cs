using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using Wordle.Application.Interfaces;
using Wordle.Infrastructure.Persistence;

namespace Tests
{
    [TestFixture]
    public class WordRepositoryTests
    {
        private WordRepository _repository;
        private MockAssetService _assetService;
        private MockLogService _logService;
        private MockProjectConfigService _configService;
        private MockEventBus _eventBus;
        private MockLocalizationService _localizationService;

        [SetUp]
        public void SetUp()
        {
            _assetService = new MockAssetService();
            _logService = new MockLogService();
            _configService = new MockProjectConfigService();
            _eventBus = new MockEventBus();
            _localizationService = new MockLocalizationService();
            _repository = new WordRepository(_assetService, _logService, _configService, _eventBus, _localizationService);
        }

        #region Constructor Tests

        [Test]
        public void Constructor_WithDependencies_CreatesInstance()
        {
            var repository = new WordRepository(_assetService, _logService, _configService, _eventBus, _localizationService);

            Assert.IsNotNull(repository);
            Assert.IsFalse(repository.IsInitialized);
        }

        [Test]
        public void Constructor_NullAssetService_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new WordRepository(null, _logService, _configService, _eventBus, _localizationService));
        }

        [Test]
        public void Constructor_NullLogService_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new WordRepository(_assetService, null, _configService, _eventBus, _localizationService));
        }

        #endregion

        #region InitializeAsync Tests

        [Test]
        public async Task InitializeAsync_LoadsWordLists()
        {
            _assetService.TargetWordsText = "HELLO\nWORLD\nGAMES";
            _assetService.ValidGuessWordsText = "TESTS\nWORDS\nGAMES";

            await _repository.InitializeAsync();

            Assert.IsTrue(_repository.IsInitialized);
        }

        [Test]
        public async Task InitializeAsync_AlreadyInitialized_DoesNotReinitialize()
        {
            _assetService.TargetWordsText = "HELLO\nWORLD";
            _assetService.ValidGuessWordsText = "TESTS\nWORDS";

            await _repository.InitializeAsync();
            var firstInitTime = _assetService.LoadCallCount;

            await _repository.InitializeAsync();

            Assert.AreEqual(firstInitTime, _assetService.LoadCallCount);
        }

        [Test]
        public void InitializeAsync_AssetLoadFails_ThrowsException()
        {
            _assetService.ShouldThrowOnLoad = true;

            Assert.ThrowsAsync<Exception>(async () => await _repository.InitializeAsync());
        }

        [Test]
        public async Task InitializeAsync_AssetLoadFails_IsInitializedRemainsFalse()
        {
            _assetService.ShouldThrowOnLoad = true;

            try
            {
                await _repository.InitializeAsync();
            }
            catch (Exception)
            {
                // Expected exception
            }

            Assert.IsFalse(_repository.IsInitialized);
        }

        #endregion

        #region GetRandomTargetWordAsync Tests

        [Test]
        public async Task GetRandomTargetWordAsync_AfterInitialization_ReturnsWord()
        {
            _assetService.TargetWordsText = "HELLO\nWORLD\nGAMES";
            _assetService.ValidGuessWordsText = "TESTS";
            await _repository.InitializeAsync();

            var word = await _repository.GetRandomTargetWordAsync();

            Assert.IsNotNull(word);
            Assert.AreEqual(5, word.Value.Length);
        }

        [Test]
        public void GetRandomTargetWordAsync_NotInitialized_ThrowsInvalidOperationException()
        {
            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _repository.GetRandomTargetWordAsync());
        }

        [Test]
        public async Task GetRandomTargetWordAsync_EmptyWordList_ThrowsInvalidOperationException()
        {
            _assetService.TargetWordsText = "";
            _assetService.ValidGuessWordsText = "TESTS";
            await _repository.InitializeAsync();

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _repository.GetRandomTargetWordAsync());
        }

        [Test]
        public async Task GetRandomTargetWordAsync_MultipleCalls_ReturnsWords()
        {
            _assetService.TargetWordsText = "HELLO\nWORLD\nGAMES\nTESTS\nWORDS";
            _assetService.ValidGuessWordsText = "TESTS";
            await _repository.InitializeAsync();

            var word1 = await _repository.GetRandomTargetWordAsync();
            var word2 = await _repository.GetRandomTargetWordAsync();
            var word3 = await _repository.GetRandomTargetWordAsync();

            Assert.IsNotNull(word1);
            Assert.IsNotNull(word2);
            Assert.IsNotNull(word3);
        }

        #endregion

        #region GetValidGuessWordsAsync Tests

        [Test]
        public async Task GetValidGuessWordsAsync_AfterInitialization_ReturnsWords()
        {
            _assetService.TargetWordsText = "HELLO";
            _assetService.ValidGuessWordsText = "TESTS\nWORDS\nGAMES";
            await _repository.InitializeAsync();

            var words = await _repository.GetValidGuessWordsAsync();

            Assert.IsNotNull(words);
            Assert.AreEqual(3, words.Length);
        }

        [Test]
        public void GetValidGuessWordsAsync_NotInitialized_ThrowsInvalidOperationException()
        {
            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _repository.GetValidGuessWordsAsync());
        }

        [Test]
        public async Task GetValidGuessWordsAsync_EmptyWordList_ThrowsInvalidOperationException()
        {
            _assetService.TargetWordsText = "HELLO";
            _assetService.ValidGuessWordsText = "";
            await _repository.InitializeAsync();

            Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _repository.GetValidGuessWordsAsync());
        }

        #endregion

        #region Word Parsing Tests

        [Test]
        public async Task ParseWordList_FiltersInvalidLengthWords()
        {
            _assetService.TargetWordsText = "HELLO\nHI\nWORLD\nTEST";
            _assetService.ValidGuessWordsText = "TESTS";
            await _repository.InitializeAsync();

            var word = await _repository.GetRandomTargetWordAsync();

            Assert.IsNotNull(word);
            Assert.AreEqual(5, word.Value.Length);
        }

        [Test]
        public async Task ParseWordList_TrimsWhitespace()
        {
            _assetService.TargetWordsText = "  HELLO  \n  WORLD  ";
            _assetService.ValidGuessWordsText = "TESTS";
            await _repository.InitializeAsync();

            var word = await _repository.GetRandomTargetWordAsync();

            Assert.IsNotNull(word);
            Assert.IsFalse(word.Value.Contains(" "));
        }

        [Test]
        public async Task ParseWordList_ConvertsToUpperCase()
        {
            _assetService.TargetWordsText = "hello\nworld";
            _assetService.ValidGuessWordsText = "tests";
            await _repository.InitializeAsync();

            var word = await _repository.GetRandomTargetWordAsync();

            Assert.IsNotNull(word);
            Assert.IsTrue(word.Value == word.Value.ToUpper());
        }

        #endregion

        #region Test Helper Classes

        private class MockAssetService : IAssetService
        {
            public string TargetWordsText { get; set; } = "HELLO\nWORLD\nGAMES";
            public string ValidGuessWordsText { get; set; } = "TESTS\nWORDS";
            public bool ShouldThrowOnLoad { get; set; }
            public int LoadCallCount { get; private set; }

            public UniTask<T> LoadAssetAsync<T>(string address)
            {
                LoadCallCount++;

                if (ShouldThrowOnLoad)
                {
                    throw new Exception("Mock asset load failed");
                }

                if (typeof(T) == typeof(TextAsset))
                {
                    // Support addresses that may be prefixed by language code, e.g. "en/TargetWords".
                    string text;
                    if (!string.IsNullOrEmpty(address) && address.EndsWith("TargetWords", StringComparison.OrdinalIgnoreCase))
                    {
                        text = TargetWordsText;
                    }
                    else if (!string.IsNullOrEmpty(address) && address.EndsWith("ValidGuessWords", StringComparison.OrdinalIgnoreCase))
                    {
                        text = ValidGuessWordsText;
                    }
                    else
                    {
                        // Fallback: if address contains "TargetWords" anywhere, use target list; otherwise use valid guess list.
                        text = !string.IsNullOrEmpty(address) && address.IndexOf("TargetWords", StringComparison.OrdinalIgnoreCase) >= 0
                            ? TargetWordsText
                            : ValidGuessWordsText;
                    }

                    var textAsset = new TextAsset(text);
                    return UniTask.FromResult((T)(object)textAsset);
                }

                throw new NotImplementedException();
            }

            public UniTask<IList<T>> LoadAssetsAsync<T>(IEnumerable<string> addresses, Action<T> callback = null)
            {
                throw new NotImplementedException();
            }

            public void ReleaseAsset<T>(T asset)
            {
                // No-op for mock
            }
        }

        private class MockLogService : ILogService
        {
            public void LogDebug(string message) { }
            public void LogInfo(string message) { }
            public void LogWarning(string message) { }
            public void LogError(string message) { }
            public void LogException(string message, Exception exception) { }
            public void SetLogLevel(LogLevel level) { }
        }

        private class MockProjectConfigService : IProjectConfigService
        {
            public int WordLength => 5;
            public int MaxAttempts => 6;
            public string TargetWordsAddress => "TargetWords";
            public string ValidGuessWordsAddress => "ValidGuessWords";
            public string StatisticsSaveKey => "Statistics";
            public string GameStateSaveKey => "GameState";
        }

        private class MockEventBus : IEventBus
        {
            public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : struct { }
            public void Subscribe<TEvent>(Func<TEvent, UniTask> asyncHandler) where TEvent : struct { }
            public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : struct { }
            public void Unsubscribe<TEvent>(Func<TEvent, UniTask> asyncHandler) where TEvent : struct { }
            public void Publish<TEvent>(TEvent eventData) where TEvent : struct { }
            public UniTask PublishAsync<TEvent>(TEvent eventData) where TEvent : struct => UniTask.CompletedTask;
            public void Clear<TEvent>() where TEvent : struct { }
            public void ClearAll() { }
        }

        private class MockLocalizationService : ILocalizationService
        {
            public string CurrentLanguage => "en";
            public IReadOnlyList<string> AvailableLanguages => new List<string> { "en" };
            public string GetString(string key) => key;
            public string GetString(string key, Dictionary<string, string> parameters) => key;
            public void SetLanguage(string languageCode) { }
            public string GetLanguageDisplayName(string languageCode) => "English";
        }

        #endregion
    }
}
