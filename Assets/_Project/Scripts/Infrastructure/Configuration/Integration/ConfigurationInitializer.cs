using Wordle.Application.Interfaces;
using Wordle.Infrastructure.Configuration;

namespace Wordle.Infrastructure.Integration
{
    public static class ConfigurationInitializer
    {
        public static void Initialize(IDependencyContainer container)
        {
            var config = ProjectConfigFactory.CreateDefault();
            Initialize(container, config);
        }

        public static void Initialize(IDependencyContainer container, ProjectConfig config)
        {
            var logService = container.Resolve<ILogService>();

            var configRegistry = new ConfigRegistry(logService);
            configRegistry.Register(config);
            container.RegisterSingleton<IConfigRegistry>(configRegistry);

            var configService = new ProjectConfigService(config, logService);
            container.RegisterSingleton<IProjectConfigService>(configService);
        }

        public static void RegisterKeyboardLayoutProvider(IDependencyContainer container)
        {
            var localizationService = container.Resolve<ILocalizationService>();
            var keyboardLayoutProvider = new KeyboardLayoutProvider(localizationService);
            container.RegisterSingleton<IKeyboardLayoutProvider>(keyboardLayoutProvider);
        }
    }
}
