using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Wordle.Application.Events;
using Wordle.Application.Interfaces;

namespace Wordle.Application.Commands
{
    /// <summary>
    /// Command: Delete a letter from the current guess.
    /// Publishes a LetterDeletedEvent for UI updates.
    /// </summary>
    public class DeleteLetterCommand : ICommand
    {
        private readonly IEventBus _eventBus;
        private readonly int _row;
        private readonly int _position;

        public string CommandName => "DeleteLetter";
        public CommandPriority Priority => CommandPriority.Normal;

        public DeleteLetterCommand(IEventBus eventBus, int row, int position)
        {
            _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
            _row = row;
            _position = position;
        }

        public bool CanExecute()
        {
            return _row >= 0 && _position >= 0 && _position < 5;
        }

        public UniTask ExecuteAsync(CancellationToken ct)
        {
            _eventBus.Publish(new LetterDeletedEvent(_row, _position));
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
