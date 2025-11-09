using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Wordle.Application.Events;
using Wordle.Application.Interfaces;

namespace Wordle.Application.Commands
{
    /// <summary>
    /// Command: Enter a letter into the current guess.
    /// Publishes a LetterEnteredEvent for UI updates.
    /// </summary>
    public class EnterLetterCommand : ICommand
    {
        private readonly IEventBus _eventBus;
        private readonly char _letter;
        private readonly int _row;
        private readonly int _position;

        public string CommandName => "EnterLetter";
        public CommandPriority Priority => CommandPriority.Normal;

        public EnterLetterCommand(IEventBus eventBus, char letter, int row, int position)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _letter = letter;
            _row = row;
            _position = position;
        }

        public bool CanExecute()
        {
            return char.IsLetter(_letter) && _row >= 0 && _position >= 0 && _position < 5;
        }

        public UniTask ExecuteAsync(CancellationToken ct)
        {
            _eventBus.Publish(new LetterEnteredEvent(_letter, _row, _position));
            return UniTask.CompletedTask;
        }

        public void OnComplete()
        {
        }

        public void OnFailed(Exception ex)
        {
        }
    }
}
