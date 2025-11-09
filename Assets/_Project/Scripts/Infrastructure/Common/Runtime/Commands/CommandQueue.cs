using System;
using System.Collections.Generic;
using System.Linq;
using Wordle.Application.Interfaces;

namespace Wordle.Infrastructure.Common.Commands
{
    /// <summary>
    /// Priority-based command queue implementation.
    /// Commands are organized by priority (lower value = higher priority).
    /// </summary>
    public class CommandQueue : ICommandQueue
    {
        private readonly Dictionary<int, Queue<ICommand>> _queues = new ();
        private readonly object _lock = new object();
        private int _totalCommandsCount = 0;

        public bool HasCommands => _totalCommandsCount > 0;
        public int TotalCommandsCount => _totalCommandsCount;

        public void Enqueue(ICommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            lock (_lock)
            {
                int priority = (int) command.Priority;

                if (!_queues.ContainsKey(priority))
                {
                    _queues[priority] = new Queue<ICommand>();
                }

                _queues[priority].Enqueue(command);
                _totalCommandsCount++;
            }
        }

        public ICommand Dequeue()
        {
            lock (_lock)
            {
                if (_totalCommandsCount == 0)
                {
                    return null;
                }

                // Get the queue with the lowest priority value (highest priority)
                var priorityKey = _queues.Keys.Min();
                var queue = _queues[priorityKey];

                var command = queue.Dequeue();
                _totalCommandsCount--;

                // Remove empty queue
                if (queue.Count == 0)
                {
                    _queues.Remove(priorityKey);
                }

                return command;
            }
        }

        public ICommand PeekOrNull()
        {
            lock (_lock)
            {
                if (_totalCommandsCount == 0)
                {
                    return null;
                }

                // Get the queue with the lowest priority value (highest priority)
                var priorityKey = _queues.Keys.Min();
                var queue = _queues[priorityKey];

                return queue.Peek();
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _queues.Clear();
                _totalCommandsCount = 0;
            }
        }

        public IEnumerable<ICommand> GetAllCommands()
        {
            lock (_lock)
            {
                var allCommands = new List<ICommand>();

                foreach (var priority in _queues.Keys.OrderBy(k => k))
                {
                    allCommands.AddRange(_queues[priority]);
                }

                return allCommands;
            }
        }
    }
}