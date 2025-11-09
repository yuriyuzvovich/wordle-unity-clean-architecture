using System;
using System.Collections.Generic;

namespace Wordle.Application.Interfaces
{
    /// <summary>
    /// Priority-based command queue.
    /// Commands are organized by priority and executed in order.
    /// </summary>
    public interface ICommandQueue
    {
        bool HasCommands { get; }
        int TotalCommandsCount { get; }

        void Enqueue(ICommand command);
        ICommand Dequeue();
        ICommand PeekOrNull();

        void Clear();
        IEnumerable<ICommand> GetAllCommands();
    }
}