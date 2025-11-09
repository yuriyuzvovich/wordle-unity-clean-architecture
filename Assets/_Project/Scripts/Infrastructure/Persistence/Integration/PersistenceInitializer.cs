using Cysharp.Threading.Tasks;
using Wordle.Application.Events;
using Wordle.Application.Interfaces;
using Wordle.Application.UseCases;
using Wordle.Core.Interfaces;
using Wordle.Core.Services;
using Wordle.Infrastructure.Persistence;

namespace Wordle.Infrastructure.Integration
{
    public static class PersistenceInitializer
    {
        public static void Initialize(IDependencyContainer container)
        {
            var logService = container.Resolve<ILogService>();
            var configService = container.Resolve<IProjectConfigService>();

            var playerPrefsService = new PlayerPrefsLocalStorageService();
            container.RegisterSingleton<ILocalStorageService>(playerPrefsService);

            var gameStateRepository = new GameStateRepository(logService, configService, playerPrefsService);
            container.RegisterSingleton<IGameStateRepository>(gameStateRepository);

            var statisticsRepository = new StatisticsRepository(logService, configService, playerPrefsService);
            container.RegisterSingleton<IStatisticsRepository>(statisticsRepository);

            var languageRepository = new LanguageRepository(logService, playerPrefsService);
            container.RegisterSingleton<ILanguageRepository>(languageRepository);

            var themeRepository = new ThemeRepository(logService, playerPrefsService);
            container.RegisterSingleton<IThemeRepository>(themeRepository);

            var eventBus = container.Resolve<IEventBus>();
            var localizationService = new LocalizationService(logService, languageRepository, eventBus);
            container.RegisterSingleton<ILocalizationService>(localizationService);

            var themeService = new ThemeService(logService, themeRepository, eventBus);
            container.RegisterSingleton<IThemeService>(themeService);

            var assetService = container.Resolve<IAssetService>();
            var wordRepository = new WordRepository(assetService, logService, configService, eventBus, localizationService);
            container.RegisterSingleton<IWordRepository>(wordRepository);

            var gameRules = new GameRules();
            container.RegisterSingleton<IGameRules>(gameRules);

            container.RegisterTransient<IGuessEvaluator, GuessEvaluator>();

            var setThemeUseCase = new SetThemeUseCase(themeService, logService);
            container.RegisterSingleton<SetThemeUseCase>(setThemeUseCase);
            var getCurrentThemeUseCase = new GetCurrentThemeUseCase(themeService, logService);
            container.RegisterSingleton<GetCurrentThemeUseCase>(getCurrentThemeUseCase);
        }

        public static async UniTask InitializeAsync(IDependencyContainer container)
        {
            var logService = container.Resolve<ILogService>();
            var wordRepository = container.Resolve<IWordRepository>();
            var eventBus = container.Resolve<IEventBus>();

            // Initialize word repository to load word lists
            logService.LogInfo("Initializing Word Repository...");
            await wordRepository.InitializeAsync();

            // Get valid words and create WordValidator
            var validWords = await wordRepository.GetValidGuessWordsAsync();
            var wordValidator = new WordValidator(validWords);
            container.RegisterSingleton<IWordValidator>(wordValidator);

            logService.LogInfo("Word Validator initialized with word list.");

            // Subscribe to WordsReloadedEvent to update WordValidator when language changes
            eventBus.Subscribe<WordsReloadedEvent>(async evt =>
            {
                logService.LogInfo($"PersistenceInitializer: Updating WordValidator for language '{evt.LanguageCode}'");
                var updatedWords = await wordRepository.GetValidGuessWordsAsync();
                wordValidator.UpdateWords(updatedWords);
                logService.LogInfo($"WordValidator updated with {evt.ValidWordsCount} words for '{evt.LanguageCode}'");
            });

            // Register KeyboardLayoutProvider after LocalizationService is available
            var localizationService = container.Resolve<ILocalizationService>();
            var keyboardLayoutProvider = new Wordle.Infrastructure.Configuration.KeyboardLayoutProvider(localizationService);
            container.RegisterSingleton<IKeyboardLayoutProvider>(keyboardLayoutProvider);
            logService.LogInfo("KeyboardLayoutProvider registered.");
        }
    }
}
