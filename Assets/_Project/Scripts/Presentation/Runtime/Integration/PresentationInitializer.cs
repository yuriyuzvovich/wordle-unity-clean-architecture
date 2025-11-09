using Cysharp.Threading.Tasks;
using Wordle.Application.Interfaces;
using Wordle.Core.Interfaces;
using Wordle.Presentation.Controllers;
using Wordle.Presentation.UI;
using Wordle.Presentation.UI.Modals;

namespace Wordle.Presentation.Integration
{
    public static class PresentationInitializer
    {
        public static async UniTask InitializeServicesAsync(IDependencyContainer container)
        {
            var canvasManager = container.Build<CanvasManager>();
            container.RegisterSingleton<ICanvasManager>(canvasManager);

            var modalManager = container.Build<ModalManager>();
            container.RegisterSingleton<IModalManager>(modalManager);

            var inputController = container.Build<InputController>();
            inputController.Initialize();
            container.RegisterSingleton(inputController);

            var gameController = container.Build<GameController>();
            var wordRepository = container.Resolve<IWordRepository>();
            var randomWord = await wordRepository.GetRandomTargetWordAsync();
            gameController.Initialize(autoStartGame: true, testWord: randomWord.Value);
            container.RegisterSingleton(gameController);
        }

        public static async UniTask InitializeUIAsync(IDependencyContainer container)
        {
            var canvasManager = container.Resolve<ICanvasManager>();
            await canvasManager.InitializeAsync();

            var modalManager = container.Resolve<IModalManager>();
            await modalManager.InitializeAsync();
        }
    }
}
