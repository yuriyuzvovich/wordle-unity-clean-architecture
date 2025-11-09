using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Wordle.Application.Interfaces;
using Wordle.Application.UseCases;

namespace Wordle.Application.Commands
{
    /// <summary>
    /// Command: Start a new game.
    /// Demonstrates the command pattern with use case execution.
    /// </summary>
    public class StartGameCommand : ICommand
    {
        private readonly StartGameUseCase _useCase;
        private readonly string _targetWord;

        public string CommandName => "StartGame";
        public CommandPriority Priority => CommandPriority.High;

        public StartGameCommand(StartGameUseCase useCase, string targetWord)
        {
            _useCase = useCase ?? throw new ArgumentNullException(nameof(useCase));
            _targetWord = targetWord;
        }

        public bool CanExecute()
        {
            return !string.IsNullOrEmpty(_targetWord);
        }

        public async UniTask ExecuteAsync(CancellationToken ct)
        {
            //UnityEngine.Debug.Log($"Executing {CommandName} command...");

            var result = await _useCase.ExecuteAsync(_targetWord);

            if (!result.Success)
            {
                throw new InvalidOperationException(result.Message);
            }

            //UnityEngine.Debug.Log($"{CommandName} command result: {result.Message}");
        }

        public void OnComplete()
        {
            //UnityEngine.Debug.Log($"{CommandName} command completed successfully.");
        }

        public void OnFailed(Exception ex)
        {
            //UnityEngine.Debug.LogError($"{CommandName} command failed: {ex.Message}");
        }
    }
}
