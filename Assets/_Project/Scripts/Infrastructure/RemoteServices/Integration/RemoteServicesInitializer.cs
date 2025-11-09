using Wordle.Application.Interfaces;
using Wordle.Infrastructure.RemoteServices;

namespace Wordle.Infrastructure.Integration
{
    public static class RemoteServicesInitializer
    {
        public static void Initialize(IDependencyContainer container)
        {
            var logService = container.Resolve<ILogService>();

            var analyticsService = new PlaceholderAnalyticsService(logService);
            container.RegisterSingleton<IAnalyticsService>(analyticsService);
        }
    }
}
