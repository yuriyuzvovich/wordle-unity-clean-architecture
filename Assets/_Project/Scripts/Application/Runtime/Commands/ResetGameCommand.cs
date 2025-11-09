using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Wordle.Application.Interfaces;
using Wordle.Application.UseCases;

namespace Wordle.Application.Commands
{
    /// <summary>
    /// Command: Reset the game and start a new one.
    /// Executes the ResetGameUseCase.
    /// </summary>
    public class ResetGameCommand : ICommand
    {
        private readonly ResetGameUseCase _useCase;

        public string CommandName => "ResetGame";
        public CommandPriority Priority => CommandPriority.High;

        public ResetGameCommand(ResetGameUseCase useCase)
        {
            _useCase = useCase ?? throw new ArgumentNullException(nameof(useCase));
        }

        public bool CanExecute()
        {
            return true;
        }

        public async UniTask ExecuteAsync(CancellationToken ct)
        {
            var result = await _useCase.ExecuteAsync();

            if (!result.Success)
            {
                throw new InvalidOperationException(result.Message);
            }
        }

        public void OnComplete()
        {
        }

        public void OnFailed(Exception ex)
        {
        }
    }
}
