using Wordle.Application.Attributes;
using Wordle.Application.Interfaces;
using Wordle.Infrastructure.Common.DI;

namespace Wordle.Presentation.Integration
{
    public class MainSceneInitializer : InjectableMonoBehaviour
    {
        [Inject] ILogService _logService;

        private async void Start()
        {
            var container = ProjectContext.Container;
            if (container == null)
            {
                _logService.LogError("MainSceneInitializer: ProjectContext.Container is null. Ensure Bootstrapper has initialized first.");
                return;
            }

            _logService?.LogInfo("MainSceneInitializer: Starting UI initialization in Main scene...");

            await PresentationInitializer.InitializeServicesAsync(container);
            _logService?.LogInfo("MainSceneInitializer: Presentation layer registered.");

            await PresentationInitializer.InitializeUIAsync(container);
            _logService?.LogInfo("MainSceneInitializer: UI components loaded successfully.");
        }
    }
}