using System;
using Cysharp.Threading.Tasks;

namespace Wordle.Application.Interfaces
{
    /// <summary>
    /// Base interface for all commands.
    /// Commands encapsulate actions that can be queued and executed.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Unique name/identifier for this command.
        /// </summary>
        string CommandName { get; }

        /// <summary>
        /// Priority for queue ordering (lower value = higher priority).
        /// </summary>
        CommandPriority Priority { get; }

        /// <summary>
        /// Execute the command asynchronously.
        /// </summary>
        UniTask ExecuteAsync(System.Threading.CancellationToken ct);

        /// <summary>
        /// Check if the command can be executed (validation).
        /// </summary>
        bool CanExecute();

        /// <summary>
        /// Called when the command completes successfully.
        /// </summary>
        void OnComplete();

        /// <summary>
        /// Called when the command fails with an exception.
        /// </summary>
        void OnFailed(Exception ex);
    }

    /// <summary>
    /// Command priority levels.
    /// Lower value = higher priority (executed first).
    /// </summary>
    public enum CommandPriority
    {
        /// <summary>Critical commands that must execute immediately (e.g., pause, quit)</summary>
        Critical = 0,

        /// <summary>High priority commands (e.g., save game, user input)</summary>
        High = 10,

        /// <summary>Normal priority commands (e.g., submit guess, start game)</summary>
        Normal = 50,

        /// <summary>Low priority commands (e.g., animations, UI updates)</summary>
        Low = 100,

        /// <summary>Background tasks (e.g., analytics, prefetching)</summary>
        Background = 200
    }
}
