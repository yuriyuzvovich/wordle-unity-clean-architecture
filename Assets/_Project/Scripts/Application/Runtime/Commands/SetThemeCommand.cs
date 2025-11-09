using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Wordle.Application.Interfaces;
using Wordle.Application.UseCases;
using Wordle.Core.Theme;

namespace Wordle.Application.Commands
{
    public class SetThemeCommand : ICommand
    {
        private readonly SetThemeUseCase _useCase;
        private readonly ThemeType _themeType;

        public string CommandName => "SetTheme";
        public CommandPriority Priority => CommandPriority.Normal;

        public SetThemeCommand(SetThemeUseCase useCase, ThemeType themeType)
        {
            _useCase = useCase ?? throw new ArgumentNullException(nameof(useCase));
            _themeType = themeType;
        }

        public bool CanExecute()
        {
            return Enum.IsDefined(typeof(ThemeType), _themeType);
        }

        public async UniTask ExecuteAsync(CancellationToken ct)
        {
            _useCase.Execute(_themeType);
            await UniTask.CompletedTask;
        }

        public void OnComplete()
        {
        }

        public void OnFailed(Exception ex)
        {
        }
    }
}
